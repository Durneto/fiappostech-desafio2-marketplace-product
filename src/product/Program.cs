using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductApi.Domains.Interfaces;
using ProductApi.Helpers.Middlewares;
using ProductApi.Infra.Repositorys.Base;
using ProductApi.Infra.Repositorys.Redis;
using ProductApi.Infra.Repositorys.SqlServer;
using ProductApi.Services;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var stringConexao = configuration.GetValue<string>("ConnectionStringSQL");
builder.Services.AddScoped<IDbConnection>((conexao) => new SqlConnection(stringConexao));
builder.Services.AddScoped<IUow, Uow>();

builder.Services.AddRedis(configuration);

builder.Services.AddLogging(builder => builder.AddConsole());

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Markplace", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Adiciona suporte para passar o token JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

var key = Encoding.ASCII.GetBytes(configuration["SecretJWT"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Somente para desenvolvimento, mude para true em produção
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false, // Você pode configurar isso dependendo das suas necessidades
            ValidateAudience = false // Você pode configurar isso dependendo das suas necessidades
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Produto API V1");
    });
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseLoggingApi();

app.MapControllers();

app.Run();
