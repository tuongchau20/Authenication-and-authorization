﻿using Authorize.Data;
using Authorize.Helper;
using Authorize.Model;
using Authorize.Repositories;
using Authorize.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("MyDbConnection");
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<TokenServices>();
builder.Services.AddScoped<GenericRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSettings"));
var secretKey = builder.Configuration["AppSettings:SecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization(options =>
{
    // Policy cho Admin: có thể làm mọi thứ
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole(UserConstants.Roles.Admin));

    // Policy cho Manager: chỉ có thể thực hiện các hành động liên quan đến quản lý User
    options.AddPolicy("ManagerPolicy", policy =>
        policy.RequireRole(UserConstants.Roles.Admin,UserConstants.Roles.Manager));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
