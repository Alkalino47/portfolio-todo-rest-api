using System.Text;
using portfolio_todo_rest_api.Application.Categories;
using portfolio_todo_rest_api.Application.Todos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTests;

public sealed class TestApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.ValidIssuer = JwtTokenFactory.Issuer;
                options.TokenValidationParameters.ValidAudience = JwtTokenFactory.Audience;
                options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(JwtTokenFactory.SigningKey));
            });

            services.RemoveAll<ITodoService>();
            services.RemoveAll<ICategoryService>();

            services.AddSingleton<TestDataStore>();
            services.AddScoped<ITodoService, FakeTodoService>();
            services.AddScoped<ICategoryService, FakeCategoryService>();
        });
    }
}
