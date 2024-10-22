using Customer.API.Services.Interfaces;

namespace Customer.API.Controllers;

public static class CustomersController
    {
        public static void MapCustomersAPI(this WebApplication app)
        {
            app.MapGet("/api/customers/{username}", async (string username, ICustomerService customerService) =>
            {
                var customer = await customerService.GetCustomerByUsernameAsync(username);
                return customer != null ? Results.Ok(customer) : Results.NotFound();
            });

            //app.MapGet("/api/customers", async (ICustomerService customerService) =>
            //{
            //    var customers = await customerService.GetCustomersAsync();
            //    return Results.Ok(customers);
            //});

            //app.MapPost("api/customers", async (Entities.Customer customer, ICustomerRepository customerRepository) =>
            //{
            //    await customerRepository.CreateAsync(customer);
            //    await customerRepository.SaveChangeAsync();
            //});

            //app.MapDelete("api/customers/{id}", async (int id, ICustomerRepository customerRepository) =>
            //{
            //    var customer = await customerRepository.FindByCondition(x => x.Id.Equals(id)).SingleOrDefaultAsync();
            //    if (customer == null) return Results.NotFound();

            //    await customerRepository.DeleteAsync(customer);
            //    await customerRepository.SaveChangeAsync();
            //    return Results.NoContent();
            //});

            //app.MapPut("api/customers/{id}", async (int id, Entities.Customer customer, ICustomerRepository customerRepository) =>
            //{
            //    var existedCustomer = await customerRepository.FindByCondition(x => x.Id.Equals(id)).SingleOrDefaultAsync();
            //    if (existedCustomer == null) return Results.NotFound();
            //    customer.Id = existedCustomer.Id;
            //    await customerRepository.UpdateAsync(customer);
            //    await customerRepository.SaveChangeAsync();
            //    return Results.Ok(customer);
            //});
        }
    }