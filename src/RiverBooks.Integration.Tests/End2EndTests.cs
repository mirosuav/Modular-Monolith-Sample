using Bogus;
using FluentAssertions;
using RiverBooks.Books.Contracts;
using RiverBooks.Users.Contracts;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RiverBooks.Integration.Tests;

public class End2EndTests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture _fixture;
    private readonly HttpClient _httpClient;
    private readonly Faker _faker = new Faker();

    public End2EndTests(ApiFixture fixture)
    {
        _fixture = fixture;
        _httpClient = _fixture.CreateClient();
    }

    [Fact]
    public async Task Login2OrderTest()
    {
        // CreateUser
        var userLogin = await CreateUser();

        // Login user
        var userToken = await LoginUser(userLogin);

        // Set user auth token
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        // Get All books
        var allBooks = await GetAllBooks();

    }

    private async Task<UserLoginRequest> CreateUser()
    {
        // Arrange
        var userEmail = _faker.Person.Email;
        var password = _faker.Hacker.Phrase();
        var request = new CreateUserRequest(userEmail, password);

        // Act
        var result = await _httpClient.PostAsync("/users", request.ToStringContentUtf());

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();

        // Extra Validation check
        var invalidResult = await _httpClient.PostAsync("/users", request.ToStringContentUtf());
        invalidResult.StatusCode.Should().Be(HttpStatusCode.Conflict);

        return new UserLoginRequest(userEmail, password);
    }

    private async Task<string> LoginUser(UserLoginRequest userLogin)
    {
        // Act
        var result = await _httpClient.PostAsync("/users/login", userLogin.ToStringContentUtf());

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var accessToken = await result.Content.ReadAsStringAsync();

        accessToken.Should().NotBeNullOrWhiteSpace();

        return accessToken;
    }

    private async Task<List<BookDto>> GetAllBooks()
    {
        // Act
        var result = await _httpClient.GetAsync("/books");

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var responseStr = await result.Content.ReadAsStringAsync();
        responseStr.Should().NotBeNullOrWhiteSpace();

        var booksResponse = JsonSerializer.Deserialize<ListBooksResponse>(responseStr);

        booksResponse.Should().NotBeNull();
        booksResponse!.Books.Should().NotBeEmpty();

        return booksResponse!.Books;
    }


}
