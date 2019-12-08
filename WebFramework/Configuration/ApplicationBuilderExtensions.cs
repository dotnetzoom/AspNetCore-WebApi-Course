using Data;
using Common.Utilities;
using Services.DataInitializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WebFramework.Configuration
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseHsts(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            Assert.NotNull(app, nameof(app));
            Assert.NotNull(env, nameof(env));

            if (!env.IsDevelopment())
                app.UseHsts();
        }

        public static void IntializeDatabase(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>(); //Service locator

            //Dos not use Migrations, just Create Database with latest changes
            //dbContext.Database.EnsureCreated();
            //Applies any pending migrations for the context to the database like (Update-Database)
            dbContext.Database.Migrate();

            var dataInitializes = scope.ServiceProvider.GetServices<IDataInitializer>();
            foreach (var dataInitializer in dataInitializes)
                dataInitializer.InitializeData();
        }
    }
}
