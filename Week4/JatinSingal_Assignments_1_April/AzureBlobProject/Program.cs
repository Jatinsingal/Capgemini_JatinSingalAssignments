using Azure.Storage.Blobs;
using AzureBlobProject.Services;

namespace AzureBlobProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var blobConnection =
                builder.Configuration.GetConnectionString("BlobConnection")
                ?? builder.Configuration.GetValue<string>("BlobConnection");

            if (string.IsNullOrWhiteSpace(blobConnection))
            {
                builder.Services.AddSingleton<IContainerService, DisabledContainerService>();
                builder.Services.AddSingleton<IBlobService, DisabledBlobService>();
            }
            else
            {
                builder.Services.AddSingleton(_ => new BlobServiceClient(blobConnection));
                builder.Services.AddSingleton<IContainerService, ContainerService>();
                builder.Services.AddSingleton<IBlobService, BlobService>();
            }

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
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
