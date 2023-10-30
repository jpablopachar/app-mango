using AutoMapper;
using coupon_service;
using coupon_service.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection")));

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();

builder.Services.AddSingleton(mapper);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

/* var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new CouponProfile());
});

IMapper mapper = mapperConfig.CreateMapper();

builder.Services.AddSingleton(mapper); */

builder.Services.AddCors(o => o.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("corsapp");

app.MapControllers();

ApplyMigration();

app.Run();

void ApplyMigration()
{
    using var scope = app!.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (db.Database.GetPendingMigrations().Any()) db.Database.Migrate();
}
