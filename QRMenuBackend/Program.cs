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

    CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
    CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

    var builder = WebApplication.CreateBuilder(args);

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
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? string.Empty))
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
    builder.Services.AddAuthorization(); // Bu satırı ekleyin

// Repository ve Service ekleme
builder.Services.AddScoped<TokenService>();

builder.Services.AddScoped<IRepository<IdentityUser>, UserRepository>();
builder.Services.AddScoped<IService<IdentityUser>, UserService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Controller'ları eklemek için bu satırı ekleyin
builder.Services.AddControllers(); // Burayı eklemeyi unutmayın

// Swagger ekleme
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

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseHsts();

app.UseAuthentication(); // JWT kimlik doğrulamasını eklemek
app.UseAuthorization();   // Yetkilendirme middleware'i


// Controller'ları ekleme
app.MapControllers(); // Tüm controller uç noktalarını ekler

app.Run();
