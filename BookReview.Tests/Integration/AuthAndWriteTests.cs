using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookReview.Dto;

namespace BookReview.Tests.Integration;

public class AuthAndWriteTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthAndWriteTests(CustomWebApplicationFactory factory) => _client = factory.CreateClient();

    private static UserDto NewCredentials() =>
        new() { Username = "user" + Guid.NewGuid().ToString("N")[..8], Password = "Passw0rd!" };

    private async Task<string> RegisterAndLoginAsync()
    {
        var creds = NewCredentials();
        (await _client.PostAsJsonAsync("/api/auth/register", creds)).EnsureSuccessStatusCode();
        var login = await _client.PostAsJsonAsync("/api/auth/login", creds);
        login.EnsureSuccessStatusCode();
        return (await login.Content.ReadAsStringAsync()).Trim('"');
    }

    [Fact]
    public async Task Register_ReturnsUser_WithoutPasswordOrHash()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", NewCredentials());

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("passwordHash", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Passw0rd", body);
    }

    [Fact]
    public async Task CreateCategory_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/categories", new CategoryDto { Name = "NoAuth" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_WithToken_Returns201_ThenDeletes()
    {
        var token = await RegisterAndLoginAsync();

        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/categories")
        {
            Content = JsonContent.Create(new CategoryDto { Name = "IT-" + Guid.NewGuid().ToString("N")[..8] })
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var createResponse = await _client.SendAsync(createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);
        var created = await createResponse.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(created);

        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/categories/{created!.Id}");
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var deleteResponse = await _client.SendAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task CreateReview_WithInvalidForeignKeys_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/reviews")
        {
            Content = JsonContent.Create(new ReviewDto
            {
                Title = "t", Text = "x", Rating = 3, BookId = 999999, ReviewerId = 999999
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
