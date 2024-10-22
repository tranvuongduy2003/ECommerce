namespace Customer.API.Services.Interfaces;

public interface ICustomerService
{
    Task<Entities.Customer> GetCustomerByUsernameAsync(string username);
    Task<List<Entities.Customer>> GetCustomersAsync();
}