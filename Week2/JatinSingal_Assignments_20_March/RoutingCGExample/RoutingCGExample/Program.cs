using System;

namespace RoutingCGExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Privacy}/{id?}");

            app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Student}/{action=GetAllStudents}/{id?}");

            app.MapControllerRoute(
            name: "studsroute",
            pattern: "studs/{action=GetAllStudents}/{id?}",
            defaults: new { controller = "Student" });

            app.MapControllerRoute(
            name: "studentsingle",
            pattern: "studs/{id}",
            defaults: new { controller = "Student", action = "GetAllStudents"} );

            app.MapControllerRoute(
            name: "fewcolumns",
            pattern: "studsfew",
            defaults: new { controller = "Student", action = "fewcolumns" });

            app.Run();
        }
    } 
}
