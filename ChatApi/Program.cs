using Microsoft.AspNetCore.WebSockets;
using StackExchange.Redis;
using System.Net.WebSockets;
var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var configuration = builder.Configuration.GetConnectionString("RedisConnection");
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, builder =>
    {
        builder.WithOrigins("http://192.168.0.165:4200") // Replace with your Angular app's URL
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // Add this if you're using credentials
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Remove this line to disable HTTPS redirection
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();
app.UseWebSockets();
app.MapControllers();

app.Run();