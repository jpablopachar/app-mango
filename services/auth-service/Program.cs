using auth_service.Data;
using auth_service.Interfaces;
using auth_service.Models;
using auth_service.Services;
using message_bus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using rabbitmq_bus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AuthDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection")));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders();

builder.Services.AddControllers();

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMessageBus, MessageBus>();
builder.Services.AddScoped<IRabbitMQMessageBus, RabbitMQMessageBus>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();

app.UseSwaggerUI(s =>
{
    if (!app.Environment.IsDevelopment())
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "Cart Api V1");

        s.RoutePrefix = string.Empty;
    }
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.Run();

void ApplyMigration()
{
    using var scope = app!.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

    if (db.Database.GetPendingMigrations().Count() > 0) db.Database.Migrate();
}