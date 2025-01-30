
using Core.Interfaces;
using MercadoPago.Config;
using Microsoft.EntityFrameworkCore;
using Payment.Data;
using Payment.Models;
using Payment.Repositories;
using Payment.Services;

namespace Payment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<PaymentDbContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

           MercadoPagoConfig.AccessToken = builder.Configuration["MercadoPago:AccessToken"];

            builder.Services.AddScoped<IRepository<Payments>, PaymentRepository>();
            builder.Services.AddScoped<PaymentService>();
            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
