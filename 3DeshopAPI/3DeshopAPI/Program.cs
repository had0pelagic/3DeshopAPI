using _3DeshopAPI.Extensions;
using _3DeshopAPI.Models.Settings;
using AutoMapper;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager config = builder.Configuration;
//builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().CreateLogger());

var origins = "AllowOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: origins,
                      builder =>
                      {
                          //builder.WithOrigins("http://localhost:3000");
                          builder.AllowAnyHeader();
                          builder.AllowAnyMethod();
                          builder.AllowAnyOrigin();
                      });
});

// Serilog configuration		
var logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();
// Register Serilog
builder.Logging.AddSerilog(logger);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "3Deshop",
        Version = "v1"
    });
    opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. bearer {token}",
    });
    opts.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.SetupServices();
builder.Services.Configure<SMTPSettings>(builder.Configuration.GetSection("SMTP"));

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddRouting(opts => opts.LowercaseUrls = true);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<Context>(opts =>
{
    var connString = builder.Configuration.GetConnectionString("MyConnectionString");
    opts.UseSqlServer(connString, options =>
    {
        options.MigrationsAssembly(typeof(Context).Assembly.FullName.Split(',')[0]);
    });
});
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(opts =>
    {
        opts.SaveToken = true;
        opts.RequireHttpsMetadata = false;
        opts.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidAudience = config["JWT:ValidAudience"],
            ValidIssuer = config["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"]))
        };
    });

var app = builder.Build();

var automapper = app.Services.GetRequiredService<IMapper>();
automapper.ConfigurationProvider.AssertConfigurationIsValid();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseCors(origins);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
