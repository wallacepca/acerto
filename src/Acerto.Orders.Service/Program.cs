using System.IdentityModel.Tokens.Jwt;
using Acerto.Orders.Service.Database;
using Acerto.Shared.Controllers;
using Acerto.Shared.Infrastructure.ServiceBus.Abstractions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi.Models;
using NetDevPack.Security.JwtExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        b => b.MigrationsAssembly(typeof(Program).Assembly.GetName().FullName))
    .UseSnakeCaseNamingConvention()
    .UseLazyLoadingProxies());

builder.Services.AddControllers(x => x.Filters.AddService<UnitOfWorkActionFilter<OrdersDbContext>>(1));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = true;
        x.SaveToken = true;
        x.IncludeErrorDetails = true;
        x.TokenValidationParameters.ValidateIssuer = false;
        x.TokenValidationParameters.SignatureValidator = (t, p) => new JwtSecurityToken(t);
        x.TokenValidationParameters.SignatureValidatorUsingConfiguration = (t, p, c) => new JsonWebToken(t);
        x.TokenValidationParameters.ValidateIssuerSigningKey = false;
        x.TokenValidationParameters.ValidateIssuer = false;
        x.TokenValidationParameters.ValidIssuers = builder.Configuration.GetSection("JwkOptions:ValidIssuers").Get<IEnumerable<string>>();
        x.SetJwksOptions(builder.Configuration.GetSection("JwkOptions").Get<JwkOptions>());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Bearer {token}",
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http
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
                Array.Empty<string>()
            }
        });
});

builder.Services.AddOrderServices();

builder.Services.AddServiceBus(options => builder.Configuration.GetSection("ServiceBus").Bind(options))
    .AddRabbitMQTransport(options => builder.Configuration.GetSection("ServiceBus:RabbitMQ").Bind(options));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
}).AddDistributedCache();

builder.Services.AddAuthSDK(options => builder.Configuration.GetSection("ApiClients:AuthApi").Bind(options));
builder.Services.AddProductsSDK(options => builder.Configuration.GetSection("ApiClients:ProductsApi").Bind(options));

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetValue<string>("ConnectionStrings:Postgres")!)
    .AddRabbitMQ(rabbitConnectionString: builder.Configuration.GetValue<string>("ServiceBus:RabbitMQ:ConnectionString")!)
    .AddRedis(builder.Configuration.GetValue<string>("ConnectionStrings:Redis")!)
    .AddDbContextCheck<OrdersDbContext>();

var app = builder.Build();

// forçando exibição para fins de demonstração
if (true || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthz");
app.UseHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = p => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.UseHealthChecksUI();

app.UpdateDatabase<OrdersDbContext>();

app.UseServiceBus();

await app.RunAsync();
