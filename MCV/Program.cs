using Entities.Data;
using Entities.Models;
using Entities.Repositories;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITIMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            //builder.Services.AddScoped<IEntityRepo<User>, EntityRepo<User>>();
            //builder.Services.AddScoped<IEntityRepo<Address>, EntityRepo<Address>>();
            //builder.Services.AddScoped<IEntityRepo<Category>, EntityRepo<Category>>();
            //builder.Services.AddScoped<IEntityRepo<Order>, EntityRepo<Order>>();
            //builder.Services.AddScoped<IEntityRepo<OrderItem>, EntityRepo<OrderItem>>();
            //builder.Services.AddScoped<IEntityRepo<Product>, EntityRepo<Product>>();


            builder.Services.AddControllersWithViews();




            builder.Services.AddDbContext<AppDbContext>(s =>
            {
                s.UseSqlServer(builder.Configuration.GetConnectionString("Connection1"));
            }, ServiceLifetime.Scoped);

            builder.Services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

/*            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });
*/

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
