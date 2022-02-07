using _3DeshopAPI.Auth.Interfaces;
using _3DeshopAPI.Auth.Services;
using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Models;
using _3DeshopAPI.Services;
using _3DeshopAPI.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics;

namespace _3DeshopAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        var message = "Internal server exception, please try again later";

                        if (contextFeature.Error is InvalidClientOperationException)
                        {
                            message = contextFeature.Error.Message;
                        }

                        await context.Response.WriteAsync(new Error
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = message
                        }.ToString());
                    }
                });
            });
        }

        public static void SetupServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
        }
    }
}
