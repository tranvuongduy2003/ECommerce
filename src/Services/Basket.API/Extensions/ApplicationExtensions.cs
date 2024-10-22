namespace Basket.API.Extensions;

public static class ApplicationExtensions
{
    public static void UseInfrastructure(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseRouting();

        app.MapGet("/", context => Task.Run(() =>
            context.Response.Redirect("/swagger/index.html")));

        app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
    }
}