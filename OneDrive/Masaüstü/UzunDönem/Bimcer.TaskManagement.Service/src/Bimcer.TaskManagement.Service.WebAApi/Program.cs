using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Bimcer.TaskManagement.Service.Business;   // AddBusinessServices()
using Bimcer.TaskManagement.Service.DataAccess; // AddDataAccessServices()
using Bimcer.TaskManagement.Service.WebAApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Serilog (isteğe bağlı, appsettings'ten okur)
// builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Bimcer Task API", Version = "v1" });
    // Bearer
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Bearer token"
    };
    opt.AddSecurityDefinition("Bearer", securityScheme);
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

// JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["SecurityKey"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

//  DbContext burada appsettings.json'dan 'Default' connection string'i alır
// DI: katman servisleri
builder.Services.AddBusinessServices();
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
//http://localhost:5261/swagger/index.html
