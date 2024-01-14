using System.Diagnostics;
using Edition.Domain.Attributes;
using Edition.Common.Extensions;
using System.Collections.Concurrent;
using Edition.Domain.Caching.Abstractions;

namespace Edition.Application.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse>
    (IRequestHandler<TRequest, TResponse> requestHandler, ICache cache)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{    
    private static readonly ConcurrentDictionary<string, bool> InProcessKeys = new();    

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Debug.WriteLine($"{requestHandler} started at :{DateTime.Now:yyyy-MM-dd HH:mm:ss}", "Interceptor");

        var methodName = nameof(IRequestHandler<TRequest, TResponse>.Handle);
        var cacheAttribute = requestHandler.GetType()
                                           .GetMethod(methodName)?
                                           .GetAttribute<CacheAttribute>();

        if (cacheAttribute is not null)
        {
            var key = request.ToString();

            //Wait if previous method is in process
            ExistKeyCheck(key!);

            var cacheResult = cache.Get<TResponse>(key!, cacheAttribute.CacheInstance);
            if (cacheResult is not null)
            {
                Debug.WriteLine($"cache is exist for {requestHandler} method log at :{DateTime.Now:yyyy-MM-dd HH:mm:ss}", "Interceptor");
                InProcessKeys.Remove(key!, out _);
                return cacheResult;
            }

            InProcessKeys.TryAdd(key!, true);

            var methodResult = await next();
            if (methodResult is null)
            {
                InProcessKeys.Remove(key!, out _);
                return default;
            }

            await cache.SetAsync(key!, methodResult, cacheAttribute.Duration, cacheAttribute.CacheInstance,
                                 cacheAttribute.Extend, cancellationToken);

            InProcessKeys.Remove(key!, out _);
            return methodResult;
        }
        return await next();
    }

    private static void ExistKeyCheck(string key)
    {
        if (InProcessKeys.ContainsKey(key)) return;
        {
            Task.Delay(50).Wait();
            var counter = 0;
            while (InProcessKeys.ContainsKey(key) && counter < 8)
            {
                Task.Delay(40).Wait();
                counter++;
            }
        }
    }
}