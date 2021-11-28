using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureProjectTemplate.API.Services.Interfaces;
using AzureProjectTemplate.API.ViewModels.Customer;
using AzureProjectTemplate.Domain.Interfaces.Notifications;
using AzureProjectTemplate.Domain.Interfaces.Repository;
using AzureProjectTemplate.Domain.Interfaces.Services;
using AzureProjectTemplate.Domain.Interfaces.UoW;
using AzureProjectTemplate.Domain.Models;
using AzureProjectTemplate.Domain.Validation.CustomerValidation;

namespace AzureProjectTemplate.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IViaCEPService _viaCEPService;
        private readonly IDomainNotification _domainNotification;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IViaCEPService viaCEPService, IDomainNotification domainNotification, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _viaCEPService = viaCEPService;
            _domainNotification = domainNotification;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerAddressViewModel>> GetAllAsync()
        {
            var customers = _mapper.Map<IEnumerable<CustomerAddressViewModel>>(await _customerRepository.GetAllAsync());

            foreach (var customer in customers)
            {
                var address = await _viaCEPService.GetByCEPAsync(customer.CEP);
                customer.Address.Id = customer.AddressId;
                customer.Address.Street = address?.Street;
                customer.Address.StreetFull = address?.StreetFull;
                customer.Address.UF = address?.UF;
            }
            return customers;
        }

        public async Task<CustomerViewModel> GetByIdAsync(CustomerIdViewModel customerVM)
        {
            return _mapper.Map<CustomerViewModel>(await _customerRepository.GetByIdAsync(customerVM.Id));
        }

        public async Task<CustomerAddressViewModel> GetAddressByIdAsync(CustomerIdViewModel customerVM)
        {
            var teste =  await _customerRepository.GetAddressByIdAsync(customerVM.Id);
            var customer = _mapper.Map<CustomerAddressViewModel>(teste);

            if (customer != null)
            {
                var address = await _viaCEPService.GetByCEPAsync(customer.CEP);

                customer.Address.Id = customer.AddressId;
                customer.Address.Street = address?.Street;
                customer.Address.StreetFull = address?.StreetFull;
                customer.Address.UF = address?.UF;
            }

            return customer;
        }

        public async Task<CustomerAddressViewModel> GetAddressByNameAsync(CustomerNameViewModel customerVM)
        {
            var customer = _mapper.Map<CustomerAddressViewModel>(await _customerRepository.GetByNameAsync(customerVM.Name));

            if (customer != null)
            {
                var address = await _viaCEPService.GetByCEPAsync(customer.CEP);

                customer.Address.Id = customer.AddressId;
                customer.Address.Street = address?.Street;
                customer.Address.StreetFull = address?.StreetFull;
                customer.Address.UF = address?.UF;
            }

            return customer;
        }

        public CustomerViewModel Add(CustomerViewModel customerVM)
        {
            CustomerViewModel viewModel = null;
            var model = _mapper.Map<Customer>(customerVM);

            var validation = new CustomerInsertValidation(_customerRepository).Validate(model);

            if (!validation.IsValid)
            {
                _domainNotification.AddNotifications(validation);
                return viewModel;
            }

            /*
             * EXEMPLO COM TRANSAÇÃO: 
             * Adicione a função "BeginTransaction()": _unitOfWork.BeginTransaction();
             * Utilize transação somente se realizar mais de uma operação no banco de dados ou banco de dados distintos
            */

            _customerRepository.Add(model);

            viewModel = _mapper.Map<CustomerViewModel>(model);

            return viewModel;
        }

        public void Update(CustomerViewModel customerVM)
        {
            var model = _mapper.Map<Customer>(customerVM);

            var validation = new CustomerUpdateValidation(_customerRepository).Validate(model);

            if (!validation.IsValid)
            {
                _domainNotification.AddNotifications(validation);
                return;
            }

            _customerRepository.Update(model);
        }

        public void Remove(CustomerViewModel customerVM)
        {
            var model = _mapper.Map<Customer>(customerVM);

            var validation = new CustomerDeleteValidation().Validate(model);

            if (!validation.IsValid)
            {
                _domainNotification.AddNotifications(validation);
                return;
            }

            _customerRepository.Remove(model);
        }
    }
}
