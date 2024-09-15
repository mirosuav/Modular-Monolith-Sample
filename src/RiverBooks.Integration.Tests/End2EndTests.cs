using Bogus;
using FluentAssertions;
using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.Users.Contracts;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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
        // CreateNewUser
        var userLogin = await CreateNewUser();

        // Login user
        var userToken = await LoginUser(userLogin);

        // Set user auth token
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken.Token);

        // Get All books
        var allBooks = await GetAllBooks();
        allBooks.Should().NotBeEmpty(); // some books pre-seeded

        // Get user adresses
        var userAdresList = await GetUserAdresses();
        userAdresList.Should().BeEmpty();

        // Get cart items
        var cartItems = await GetCartItems();
        cartItems.Should().BeEmpty();

    }

    private async Task<UserLoginRequest> CreateNewUser()
    {
        // Arrange
        var userEmail = _faker.Person.Email;
        var password = _faker.Hacker.Phrase();

        await CreateUser_ShouldReturnProperStatus(userEmail, password);

        return new UserLoginRequest(userEmail, password);
    }

    [Theory]
    [InlineData("Thomas@acme.com", "", HttpStatusCode.BadRequest)]
    [InlineData("Thomas@acme.com", " ", HttpStatusCode.BadRequest)]
    [InlineData("Thomas@acme.com", "123", HttpStatusCode.BadRequest)]
    [InlineData("Thomasacme.com", "123456", HttpStatusCode.BadRequest)]
    [InlineData("Thomas@acme@com", "123456", HttpStatusCode.BadRequest)]
    [InlineData("Thomas@acme.com@", "123456", HttpStatusCode.BadRequest)]
    [InlineData("Thomas@acme.com", "123456", HttpStatusCode.Created)]
    public async Task CreateUser_ShouldReturnProperStatus(string userEmail, string password, HttpStatusCode expectedResult = HttpStatusCode.Created)
    {
        var request = new CreateUserRequest(userEmail, password);

        // Act
        var result = await _httpClient.PostAsJsonAsync("/users", request);

        // Assert
        result.StatusCode.Should().Be(expectedResult);

        if (result.IsSuccessStatusCode)
        {
            // Extra Validation check
            var invalidResult = await _httpClient.PostAsJsonAsync("/users", request);
            invalidResult.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }

    private async Task<AuthToken> LoginUser(UserLoginRequest userLogin)
    {
        // Act
        var result = await _httpClient.PostAsJsonAsync("/users/login", userLogin);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var authToken = await result.Content.ReadFromJsonAsync<AuthToken>();
        authToken.Token.Should().NotBeNullOrWhiteSpace();

        return authToken;
    }

    private async Task<List<BookDto>> GetAllBooks()
    {
        // Act
        var result = await _httpClient.GetAsync("/books");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var booksResponse = await result.Content.ReadFromJsonAsync<ListBooksResponse>();

        booksResponse.Should().NotBeNull();
        booksResponse!.Books.Should().NotBeNull();

        return booksResponse!.Books;
    }
    
    private async Task<List<UserAddressDto>> GetUserAdresses()
    {
        // Act
        var result = await _httpClient.GetAsync("/users/addresses");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var adressesResponse = await result.Content.ReadFromJsonAsync<AddressListResponse>();

        adressesResponse.Should().NotBeNull();
        adressesResponse!.Addresses.Should().NotBeNull();

        return adressesResponse!.Addresses;
    }
    
    private async Task<List<CartItemDto>> GetCartItems()
    {
        // Act
        var result = await _httpClient.GetAsync("/cart");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var cartResponse = await result.Content.ReadFromJsonAsync<CartResponse>();

        cartResponse.Should().NotBeNull();
        cartResponse!.CartItems.Should().NotBeNull();

        return cartResponse!.CartItems;
    }

}
