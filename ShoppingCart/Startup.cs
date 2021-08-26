using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using ShoppingCart.EventFeed;
using ShoppingCart.ShoppingCart;

namespace ShoppingCart
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<IShoppingCartStore, ShoppingCartStore>();
            services.AddScoped<IEventStore, EventStore>();
            services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>()
                .AddTransientHttpErrorPolicy(p =>
                    p.WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt))));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}