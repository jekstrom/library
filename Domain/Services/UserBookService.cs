using System;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Repositories;

namespace Domain.Services
{
    public class UserBookService : IUserBookService
    {
        private readonly IUserBookRepository _repository;
        private readonly IRepository<Book> _bookRepository;

        public UserBookService(IUserBookRepository repository, IRepository<Book> bookRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        }

        public async Task<Book> CheckinBook(string username, int bookId)
        {
            Book book = await _bookRepository.Get(bookId);

            if (book.CheckedOut)
            {
                book = book.CheckIn(username);
                await _repository.RemoveUserBook(book);
                book.ResetSetCheckedOutBy(username);
            }

            return book;
        }

        public async Task<Book> CheckoutBook(string username, int bookId)
        {
            Book book = await _bookRepository.Get(bookId);

            if (!book.CheckedOut)
            {
                book = book.CheckOut(username);
                await _repository.AddUserBook(book);
            }

            return book;
        }
    }
}
