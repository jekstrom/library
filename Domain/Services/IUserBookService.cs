using Domain.Models;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IUserBookService
    {
        Task<Book> CheckoutBook(string username, int bookId);
        Task<Book> CheckinBook(string username, int bookId);
    }
}
