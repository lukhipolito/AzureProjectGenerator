using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureProjectTemplate.Domain.Interfaces.Repository;
using AzureProjectTemplate.Domain.Models;
using AzureProjectTemplate.Domain.Models.Dapper;
using AzureProjectTemplate.Infra.Context;

namespace AzureProjectTemplate.Infra.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DapperContext _dapperContext;

        public CustomerRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<IEnumerable<CustomerAddress>> GetAllAsync()
        {
            var query = @"SELECT c.Id AS id, a.Id AS addressId, c.Name AS name, c.DateCreated AS dateCreated, a.CEP AS cep
                            FROM dbo.Customer c
                            INNER JOIN dbo.Address a
                            ON c.addressId = a.Id";

            return await _dapperContext.DapperConnection.QueryAsync<CustomerAddress>(query,null,null,null,null);
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            var query = @"SELECT Id, AddressId, Name, DateCreated
                          FROM dbo.Customer c
                          WHERE c.Id = @Id";

            return (await _dapperContext.DapperConnection.QueryAsync<Customer>(query, new { Id = id })).FirstOrDefault();
        }

        public async Task<CustomerAddress> GetAddressByIdAsync(int id)
        {
            var query = @"SELECT c.Id, a.Id AS AddressId, c.Name, c.DateCreated, a.CEP
                          FROM dbo.Customer c
                          INNER JOIN dbo.Address a
                          ON c.AddressId = a.Id
                          WHERE c.Id = @Id";

            return (await _dapperContext.DapperConnection.QueryAsync<CustomerAddress>(query, new { Id = id })).FirstOrDefault();
        }

        public async Task<CustomerAddress> GetByNameAsync(string name)
        {
            var query = @"SELECT c.Id AS id, a.Id AS addressId, c.Name AS name, c.DateCreated AS dateCreated, a.CEP AS cep
                          FROM dbo.Customer c
                          INNER JOIN dbo.Address a
                          ON c.addressId = a.Id
                          WHERE c.Name = @Name";

            return (await _dapperContext.DapperConnection.QueryAsync<CustomerAddress>(query, new { Name = name })).FirstOrDefault();
        }

        public void Add(Customer obj)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Customer obj)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(Customer obj)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
