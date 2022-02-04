using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using SocialFood.API.Services;
using SocialFood.API.Settings;
using SocialFood.Data.Repository;
using SocialFood.StorageProviders;

var builder = WebApplication.CreateBuilder(args);
const bool LOCALSTORAGE = false;

var jwtSettings = Configure<JwtSettings>("JwtSettings");
string connectionString = builder.Configuration.GetConnectionString("MySQL");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Insert the Bearer Token",
        Name = HeaderNames.Authorization,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder =>
        builder.SetIsOriginAllowed(o => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

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
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey)),
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.AddAuthorization(options =>
{
    //Imposta tutti i controller per richiedere di default autenticazione
    options.FallbackPolicy = options.DefaultPolicy;
});

if (LOCALSTORAGE)
{
    builder.Services.AddFileSystemStorageProvider(options =>
    {
        options.StorageFolder = builder.Configuration.GetValue<string>("AppSettings:StorageFolder");
    });
}
else
{
    builder.Services.AddAzureStorageProvider(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("StorageConnection");
        options.ContainerName = builder.Configuration.GetValue<string>("AppSettings:ContainerName");
    });
}

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddSingleton<IAuthRepository>(_ => new AuthRepository(connectionString));
builder.Services.AddSingleton<IAccountRepository>(_ => new AccountRepository(connectionString));
builder.Services.AddSingleton<IImageRepository>(_ => new ImageRepository(connectionString));

T Configure<T>(string sectionName) where T : class
{
    var section = builder.Configuration.GetSection(sectionName);
    var settings = section.Get<T>();
    builder.Services.Configure<T>(section);

    return settings;
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

