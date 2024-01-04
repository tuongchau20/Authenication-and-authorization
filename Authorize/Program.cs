using Authorize.Data;
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
//add scoped
builder.Services.AddScoped<TokenServices>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSettings"));
var secretKey = builder.Configuration["AppSettings:SecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
//add Auhthen
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
//add authorize
builder.Services.AddAuthorization(options =>
{
    // Policy  Admin: có thể làm mọi thứ
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole(UserConstants.Roles.Admin));

    // Policy Manager: quản lý User
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
