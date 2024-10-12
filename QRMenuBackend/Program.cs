using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QRMenuBackend.Data;
using QRMenuBackend.Repositories;
using QRMenuBackend.Services;
using System.Globalization;
using System.Text;
using Microsoft.OpenApi.Models; // OpenApiInfo ve OpenApiSecurityScheme için
using Microsoft.AspNetCore.Diagnostics;

// Kültürel ayarlar
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Kestrel yapılandırması
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(1923, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

// JWT ayarlarını almak
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// JWT kimlik doğrulama eklemek
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
        };
    });


// Veritabanı bağlantısı ekleme
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity ekleme
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Authorization ekleme
builder.Services.AddAuthorization();

// Repository ve Service ekleme
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IRepository<IdentityUser>, UserRepository>();
builder.Services.AddScoped<IService<IdentityUser>, UserService>();

// CORS ekleme
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Controller'ları eklemek
builder.Services.AddControllers();

// Swagger ekleme
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API V1", Version = "v1" });

    // JWT Bearer token için güvenlik tanımı
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Ana dizinde erişim
    });
}

// Middleware sırası
app.UseCors("AllowAll");           // CORS middleware'i
app.UseHttpsRedirection();         // HTTPS yönlendirme
app.UseHsts();                     // HSTS middleware'i

app.UseAuthentication();           // JWT kimlik doğrulaması middleware'i
app.UseAuthorization();            // Yetkilendirme middleware'i

// Controller'ları ekleme
app.MapControllers();
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        await context.Response.WriteAsync($"An unexpected error occurred: {exception?.Message}");
    });
});


app.Run();
