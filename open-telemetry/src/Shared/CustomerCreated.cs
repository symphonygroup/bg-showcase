namespace Shared;

public record CustomerCreated
{
    public int Id { get; init; }
    public required string Name { get; init; }
}