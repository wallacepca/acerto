using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NetDevPack.Identity.Data;
using NetDevPack.Identity.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddIdentityEntityFrameworkContextConfiguration(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        b => b.MigrationsAssembly(typeof(Program).Assembly.GetName().FullName)));

builder.Services.AddJwtConfiguration(builder.Configuration)
    .AddNetDevPackIdentity()
    .UseJwtValidation();

builder.Services.AddMemoryCache();

builder.Services.AddIdentityConfiguration();

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
                Array.Empty<string>()
            }
        });
});

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetValue<string>("ConnectionStrings:Postgres")!)
    .AddDbContextCheck<NetDevPackAppDbContext>();

var app = builder.Build();

// forçando exibição para fins de demonstração
if (true || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthConfiguration();

app.UseJwksDiscovery();

app.MapControllers();

app.MapHealthChecks("/healthz");
app.MapHealthChecksUI();

app.UpdateDatabase<NetDevPackAppDbContext>();

await app.RunAsync();
