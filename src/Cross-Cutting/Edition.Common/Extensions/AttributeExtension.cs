using System.Reflection;

namespace Edition.Common.Extensions;

public static class AttributeExtension
{
    public static bool HasAttribute<T, TAttribute>()
    {
        return typeof(T).CustomAttributes.Any(x => x.AttributeType == typeof(TAttribute));
    }

    public static TAttribute? GetAttribute<T, TAttribute>() where T : class where TAttribute : Attribute
      => (TAttribute?)typeof(T).GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault();

    public static List<TAttribute>? GetAttributes<T, TAttribute>() where T : class where TAttribute : Attribute
      => typeof(T).GetCustomAttributes(typeof(TAttribute), false).Select(x => (TAttribute)x).ToList();

    public static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute
      => type.CustomAttributes.Any(x => x.AttributeType == typeof(TAttribute));

    public static TAttribute? GetAttribute<TAttribute>(this Type type) where TAttribute : Attribute
      => (TAttribute?)type.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault();

    public static List<TAttribute>? GetAttributes<TAttribute>(this Type type) where TAttribute : Attribute
      => type.GetCustomAttributes(typeof(TAttribute), false).Select(x => (TAttribute)x).ToList();

    public static bool HasAttribute<TAttribute>(this MethodInfo methodInfo) where TAttribute : Attribute
      => methodInfo.CustomAttributes.Any(x => x.AttributeType == typeof(TAttribute));

    public static TAttribute? GetAttribute<TAttribute>(this MethodInfo methodInfo) where TAttribute : Attribute
      => (TAttribute?)methodInfo.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault();

    public static List<TAttribute>? GetAttributes<TAttribute>(this MethodInfo methodInfo) where TAttribute : Attribute
      => methodInfo.GetCustomAttributes(typeof(TAttribute), false).Select(x => (TAttribute)x).ToList();
}