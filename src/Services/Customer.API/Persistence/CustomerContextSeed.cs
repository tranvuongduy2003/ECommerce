using Microsoft.EntityFrameworkCore;

namespace Customer.API.Persistence;

public static class CustomerContextSeed
{
    public static IHost SeedCustomerData(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var customerContext = scope.ServiceProvider.GetService<CustomerContext>();
        customerContext.Database.MigrateAsync().GetAwaiter().GetResult();

        CreateCustomer(customerContext, new Entities.Customer
        {
            UserName = "customer1",
            FirstName = "customer1",
            LastName = "customer",
            EmailAddress = "customer1@local.com"
        }).GetAwaiter().GetResult();

        CreateCustomer(customerContext, new Entities.Customer
        {
            UserName = "customer2",
            FirstName = "customer2",
            LastName = "customer",
            EmailAddress = "customer2@local.com"
        }).GetAwaiter().GetResult();

        return host;
    }

    private static async Task CreateCustomer(CustomerContext customerContext, Entities.Customer customer)
    {
        var existCustomer = await customerContext.Customers.SingleOrDefaultAsync(x => x.UserName.Equals(customer.UserName));

        if (existCustomer == null)
        {
            await customerContext.Customers.AddAsync(customer);
            await customerContext.SaveChangesAsync();
        }
    }
}