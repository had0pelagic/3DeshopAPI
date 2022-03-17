using _3DeshopAPI.Auth.Interfaces;
using _3DeshopAPI.Auth.Services;
using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Models;
using _3DeshopAPI.Services;
using _3DeshopAPI.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using PaymentAPI.Services;
using PaymentAPI.Services.Interfaces;

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

                        switch (contextFeature.Error)
                        {
                            case InvalidClientOperationException:
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                await context.Response.WriteAsync(new Error
                                {
                                    StatusCode = StatusCodes.Status400BadRequest,
                                    Message = contextFeature.Error.Message
                                }.ToString());
                                break;
                            default:
                                await context.Response.WriteAsync(new Error
                                {
                                    StatusCode = context.Response.StatusCode,
                                    Message = message
                                }.ToString());
                                break;
                        }
                    }
                });
            });
        }

        public static void SetupServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IProductDetailService, ProductDetailService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IFileService, FileService>();
        }
    }
}
