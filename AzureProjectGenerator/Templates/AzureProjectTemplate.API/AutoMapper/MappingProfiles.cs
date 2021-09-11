using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using AzureProjectTemplate.API.ViewModels.Customer;
using AzureProjectTemplate.Domain.Models;
using AzureProjectTemplate.Domain.Models.Dapper;

namespace AzureProjectTemplate.API.AutoMapper
{
    [ExcludeFromCodeCoverage]
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            #region Customer

            CreateMap<CustomerAddress, CustomerAddressViewModel>()
                .ConstructUsing(s => new CustomerAddressViewModel(
                    s.Id,
                    s.AddressId, 
                    s.Name, 
                    s.DateCreated, 
                    s.CEP, 
                    null));
            CreateMap<Customer, CustomerViewModel>()
                .ConstructUsing(s=> new CustomerViewModel(
                    s.Id,
                    s.AddressId,
                    s.Name
                )).ReverseMap();

            #endregion
        }
    }
}
