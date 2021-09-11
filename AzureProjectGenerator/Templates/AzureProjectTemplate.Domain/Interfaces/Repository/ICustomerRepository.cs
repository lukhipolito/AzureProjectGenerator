using System.Collections.Generic;
using System.Threading.Tasks;
using AzureProjectTemplate.Domain.Models;
using AzureProjectTemplate.Domain.Models.Dapper;

namespace AzureProjectTemplate.Domain.Interfaces.Repository
{
    public interface ICustomerRepository : IEntityBaseRepository<Customer>, IDapperReadRepository<Customer>
    {
        Task<IEnumerable<CustomerAddress>> GetAllAsync();
        Task<CustomerAddress> GetAddressByIdAsync(int id);
        Task<CustomerAddress> GetByNameAsync(string name);
    }
}
