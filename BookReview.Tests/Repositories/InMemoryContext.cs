using BookReview.Data;
using Microsoft.EntityFrameworkCore;

namespace BookReview.Tests.Repositories;

// Helper that builds a DataContext backed by a fresh in-memory database.
// Each call defaults to a unique database name so tests stay isolated.
internal static class InMemoryContext
{
    public static DataContext Create(string? name = null) =>
        new(new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(name ?? $"repo-{Guid.NewGuid()}")
            .Options);
}
