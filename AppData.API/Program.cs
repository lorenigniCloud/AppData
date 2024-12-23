using AppData.API.Models;
using AppData.Business;
using AppData.Business.IService;
using AppData.Infrastructures.Models;
using AppData.Infrastructures.Models.IStorage;
using AppData.Infrastructures.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Configura la stringa di connessione
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configura il contesto del database

builder.Services.AddDbContext<AppDataSqlServerContext>(options =>
options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.MigrationsAssembly("AppData.Migrations")));

// Configura ASP.NET Core Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDataSqlServerContext>()
    .AddDefaultTokenProviders();

// Aggiungi i servizi dell'applicazione
builder.Services.AddScoped<IWordService, WordService>();
builder.Services.AddScoped<IWordStorage, WordStorage>();

// Aggiungi i servizi per i controller e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Inizializzazione dei ruoli
        await RoleInitializer.InitializeRoles(services);
    }
    catch (Exception ex)
    {
        // Gestione degli errori
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Errore durante l'inizializzazione dei ruoli.");
    }
}

// Configura il pipeline di richiesta
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SupportedSubmitMethods(new[] { SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Delete });
    });
}

app.UseHttpsRedirection();

// Aggiungi autenticazione e autorizzazione
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
