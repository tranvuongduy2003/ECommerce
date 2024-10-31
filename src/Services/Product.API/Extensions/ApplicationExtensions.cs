using Infrastructure.Middlewares;

namespace Product.API.Extensions;

public static class ApplicationExtensions
{
    public static void UseInfrastructure(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseMiddleware<ErrorWrappingMiddleware>();
        app.UseAuthentication();
        app.UseRouting();
        //app.UseHttpsRedirection(); // for production only

        app.UseAuthorization();
        
        app.MapGet("/", context => Task.Run(() =>
            context.Response.Redirect("/swagger/index.html")));

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}