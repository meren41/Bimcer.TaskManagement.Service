using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Bimcer.TaskManagement.Service.Business;   // AddBusinessServices()
using Bimcer.TaskManagement.Service.DataAccess; // AddDataAccessServices()
using Bimcer.TaskManagement.Service.WebAApi.Extensions;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Bimcer Task API", Version = "v1" });

    // Bearer (JWT) – Swagger Authorize düğmesi için
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Authorization header için: **Bearer {access_token}** formatında giriniz."
    });

    // Bu kısım, tüm endpoint'lerde Authorize düğmesini aktif eder
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["SecurityKey"]!);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.FromSeconds(60)
        };
    });

// Yetkilendirme servislerini ekleyelim (UseAuthorization çağrısıyla eşleşsin)
builder.Services.AddAuthorization();

// DbContext burada appsettings.json'dan 'Default' connection string'i alır
// DI: katman servisleri
builder.Services.AddBusinessServices(builder.Configuration);
builder.Services.AddDataAccessServices(builder.Configuration);

// AutoMapper/FluentValidation vs. burada da eklenebilir
// builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Bekleyen migration'ları otomatik uygula (seed: isteğe bağlı)
await app.MigrateDatabaseAsync(seed: false);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// http://localhost:5261/swagger/index.html
