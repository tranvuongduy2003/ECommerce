using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public Task<Entities.Customer> GetCustomerByUsernameAsync(string username)
    {
        return _customerRepository.GetCustomerByUsernameAsync(username);
    }

    public Task<List<Entities.Customer>> GetCustomersAsync()
    {
        return _customerRepository.FindAll().ToListAsync();
    }
}