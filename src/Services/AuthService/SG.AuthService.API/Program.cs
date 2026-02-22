using System.Reflection;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using SG.AuthService.Application.Interfaces;
using SG.AuthService.Application.Services;
using SG.AuthService.Domain.Repositories;
using SG.AuthService.API.Middlewares;
using SG.AuthService.Infrastructure.Authentication;
using SG.AuthService.Infrastructure.Data;
using SG.AuthService.Infrastructure.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
  
  options.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<AuthDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Middleware global
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();
app.MapControllers();

app.Run();