﻿using RiverBooks.Books.Contracts;
using RiverBooks.Books.Domain;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Books.Application;

internal class BookService(IBookRepository bookRepository) : IBookService
{
    public async Task<ResultOf> CreateBookAsync(BookDto newBook)
    {
        if (await bookRepository.GetByTitleAndAuthorAsync(newBook.Title, newBook.Author) is not null)
            return Error.Conflict("Book.Exists", "Book with this title and author already exists.");

        var book = new Book(newBook.Id, newBook.Title, newBook.Author, newBook.Price);

        await bookRepository.AddAsync(book);
        await bookRepository.SaveChangesAsync();

        return true;
    }

    public async Task<ResultOf> DeleteBookAsync(Guid id)
    {
        var bookToDelete = await bookRepository.GetByIdAsync(id);

        if (bookToDelete is null)
            return Error.NotFound();

        await bookRepository.DeleteAsync(bookToDelete);
        await bookRepository.SaveChangesAsync();

        return true;
    }

    public async Task<ResultOf<BookDto>> GetBookByIdAsync(Guid id)
    {
        var book = await bookRepository.GetByIdAsync(id);

        if (book is null)
            return Error.NotFound();

        return new BookDto(book!.Id, book.Title, book.Author, book.Price);
    }

    public async Task<List<BookDto>> ListBooksAsync()
    {
        var books = (await bookRepository.ListAsync())
            .Select(book => new BookDto(book.Id, book.Title, book.Author, book.Price))
            .ToList();

        return books;
    }

    public async Task<ResultOf> UpdateBookPriceAsync(Guid bookId, decimal newPrice)
    {
        if (newPrice <= decimal.Zero)
            return Error.Validation("Invalid price", "Price must be positive number");

        var book = await bookRepository.GetByIdAsync(bookId);

        if (book is null)
            return Error.NotFound();

        book.UpdatePrice(newPrice);
        await bookRepository.SaveChangesAsync();

        return true;
    }
}