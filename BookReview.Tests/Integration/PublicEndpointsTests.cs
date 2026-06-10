using System.Net;
using System.Net.Http.Json;
using BookReview.Dto;

namespace BookReview.Tests.Integration;

public class PublicEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PublicEndpointsTests(CustomWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task GetBooks_ReturnsSeededBooks()
    {
        var books = await _client.GetFromJsonAsync<List<BookDto>>("/api/books");

        Assert.NotNull(books);
        Assert.True(books!.Count >= 3);
    }

    [Fact]
    public async Task GetAuthors_IncludesCountryName()
    {
        var authors = await _client.GetFromJsonAsync<List<AuthorDto>>("/api/authors");

        Assert.NotNull(authors);
        Assert.Contains(authors!, a => !string.IsNullOrWhiteSpace(a.CountryName));
    }

    [Fact]
    public async Task GetReviews_PopulatesBookAndReviewerIds()
    {
        var reviews = await _client.GetFromJsonAsync<List<ReviewDto>>("/api/reviews");

        Assert.NotNull(reviews);
        Assert.NotEmpty(reviews!);
        Assert.All(reviews!, r => Assert.True(r.BookId > 0 && r.ReviewerId > 0));
    }

    [Fact]
    public async Task GetBook_ReturnsNotFound_ForUnknownId()
    {
        var response = await _client.GetAsync("/api/books/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCountryByAuthor_ReturnsTheAuthorsCountry()
    {
        var authors = await _client.GetFromJsonAsync<List<AuthorDto>>("/api/authors");
        var author = authors!.First();

        var response = await _client.GetAsync($"/api/countries/authors/{author.Id}");

        response.EnsureSuccessStatusCode();
        var country = await response.Content.ReadFromJsonAsync<CountryDto>();
        Assert.NotNull(country);
        Assert.Equal(author.CountryName, country!.Name);
    }
}
