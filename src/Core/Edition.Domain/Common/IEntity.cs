namespace Edition.Domain.Common;

public interface IEntity
{
    int Id { get; set; }
}

public interface IEntity<Type>
{
    Type Id { get; set; }
}