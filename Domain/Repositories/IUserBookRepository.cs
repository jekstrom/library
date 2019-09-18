using Domain.Models;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IUserBookRepository
    {
        Task AddUserBook(Book book);
        Task RemoveUserBook(Book book);
    }
}
