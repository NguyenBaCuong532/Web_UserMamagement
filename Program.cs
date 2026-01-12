using Microsoft.EntityFrameworkCore;
using Web_App_UserManagement.Data;
using Web_App_UserManagement.Services;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add DbContext
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Add services to the container.
    builder.Services.AddScoped<IUserRepository, DbUserRepository>();

    builder.Services.AddControllersWithViews();
    
    // Add Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Ensure database is created and migrations are applied
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
            // Uncomment the line below if you want to use migrations instead of EnsureCreated
            // context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var dbLogger = services.GetRequiredService<ILogger<Program>>();
            dbLogger.LogError(ex, "An error occurred while creating or migrating the database.");
        }
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=User}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    // Log to console and event log
    Console.WriteLine($"Application failed to start: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
    }
    throw;
}
