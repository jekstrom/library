using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IRepository<T> where T : IDomainModel
    {
        Task<T> Create(T newModel);
        Task<IReadOnlyCollection<T>> Get();
        Task<T> Get(int id);
        Task<T> Get(string id);
        Task<T> Update(int id, T newModel);
        Task<bool> Delete(int id);
    }
}
