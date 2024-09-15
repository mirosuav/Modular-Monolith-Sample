using Bogus;
using FluentAssertions;
using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.Users.Contracts;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.OutputCaching;
using RiverBooks.OrderProcessing.Contracts;

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
    public async Task FullApiFlowTest()
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

        // Get user orders
        var userOrders = await GetAllUserOrders();
        userOrders.Should().BeEmpty();

        // Get cart items
        var cartItems = await GetCartItems();
        cartItems.Should().BeEmpty();

        // Get existing book by id
        var bookId = allBooks[0].Id;
        var book = await GetBookById(bookId);
        book.Should().BeEquivalentTo(allBooks[0]);

        // Create new book
        book = await CreateNewBook();
        bookId = book.Id;

        // Check new book is enlisted
        allBooks = await GetAllBooks();
        allBooks.Should().ContainEquivalentOf(book);

        // Delete new book
        await DeleteBook(bookId);

        // Check book is deleted
        allBooks = await GetAllBooks();
        allBooks.Should().NotContainEquivalentOf(book);

        // Create new book
        book = await CreateNewBook();
        bookId = book.Id;
        allBooks = await GetAllBooks();

        // Update books price
        book = book with { Price = book.Price + 9 };
        await UpdateBooksPrice(bookId, book.Price);

        // Check price updated
        var book2 = await GetBookById(bookId);
        book2.Should().BeEquivalentTo(book);

        // Add book to the cart
        var quantity = 2;
        await AddItemToCart(bookId, quantity);

        // Check book enlisted in cart
        cartItems = await GetCartItems();
        cartItems.Should().HaveCount(1);
        cartItems[0].BookId.Should().Be(bookId);
        cartItems[0].Quantity.Should().Be(2);
        cartItems[0].UnitPrice.Should().Be(book.Price);

        // Add user address
        var addressId = await AddNewUserAddress();
        var address2Id = await AddNewUserAddress();

        // Checkout cart
        var orderId = await CheckoutCart(addressId, address2Id);

        // Check cart is empty
        cartItems = await GetCartItems();
        cartItems.Should().BeEmpty();

        userOrders = await GetAllUserOrders();
        userOrders.Should().NotBeEmpty();
        userOrders[0].OrderId.Should().Be(orderId);
        userOrders[0].Total.Should().Be(quantity * book.Price);

        

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
        // Arrange
        // Reset auth header
        _httpClient.DefaultRequestHeaders.Authorization = null;

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

    private async Task<BookDto> GetBookById(Guid bookId)
    {
        // Act
        var result = await _httpClient.GetAsync($"/books/{bookId}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var book = await result.Content.ReadFromJsonAsync<BookDto>();

        book.Should().NotBeNull();
        book!.Id.Should().Be(bookId);

        return book;
    }

    private async Task UpdateBooksPrice(Guid bookId, decimal newPrice)
    {
        // Act
        var result = await _httpClient.PostAsJsonAsync($"/books/{bookId}/pricehistory", newPrice);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<BookDto> CreateNewBook()
    {
        // Arrange
        var bookCreateRequest = new CreateBookRequest
        {
            Author = _faker.Person.FullName,
            Price = (decimal)_faker.Random.Float(9, 50),
            Title = _faker.Commerce.ProductName(),
        };

        // Act
        var result = await _httpClient.PostAsJsonAsync("/books", bookCreateRequest);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var book = await result.Content.ReadFromJsonAsync<BookDto>();

        book.Should().NotBeNull();
        book!.Id.Should().NotBeEmpty();
        book.Author.Should().Be(bookCreateRequest.Author);
        book.Price.Should().Be(bookCreateRequest.Price);
        book.Title.Should().Be(bookCreateRequest.Title);

        return book;
    }

    private async Task DeleteBook(Guid bookId)
    {
        // Act
        var result = await _httpClient.DeleteAsync($"/books/{bookId}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
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

    private async Task<Guid> AddNewUserAddress()
    {
        // Arrange
        var request = new AddAddressRequest(
            _faker.Address.StreetName(),
            _faker.Address.StreetAddress(),
            _faker.Address.City(),
            _faker.Address.State(),
            _faker.Address.ZipCode(),
            _faker.Address.Country()
        );

        // Act
        var result = await _httpClient.PostAsJsonAsync("/users/addresses", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var addressId = await result.Content.ReadFromJsonAsync<Guid>();

        addressId.Should().NotBeEmpty();

        return addressId;
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

    private async Task AddItemToCart(Guid bookId, int quantity)
    {
        // Arrange
        var request = new AddCartItemRequest(bookId, quantity);

        // Act
        var result = await _httpClient.PostAsJsonAsync("/cart", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<Guid> CheckoutCart(Guid shippingAddress, Guid billingAddress)
    {
        // Arrange
        var request = new CheckoutRequest(shippingAddress, billingAddress);

        // Act
        var result = await _httpClient.PostAsJsonAsync("/cart/checkout", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await result.Content.ReadFromJsonAsync<Guid>();
        response.Should().NotBeEmpty();

        return response;
    }
    private async Task<List<OrderSummary>> GetAllUserOrders()
    {
        // Act
        var result = await _httpClient.GetAsync("/orders");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await result.Content.ReadFromJsonAsync<ListOrdersForUserResponse>();

        response.Should().NotBeNull();
        return response!.Orders;
    }

}
