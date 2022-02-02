using _3DeshopAPI.Services;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSingleton<Context>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<Context>(opts =>
{
    //opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    var connString = builder.Configuration.GetConnectionString("MyConnectionString");
    opts.UseSqlServer(connString, options =>
    {
        options.MigrationsAssembly(typeof(Context).Assembly.FullName.Split(',')[0]);
    });
});

var app = builder.Build();

var automapper = app.Services.GetRequiredService<IMapper>();
automapper.ConfigurationProvider.AssertConfigurationIsValid();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
