using AppData.API.Models;
using AppData.Business;
using AppData.Business.IService;
using AppData.Infrastructures.Models;
using AppData.Infrastructures.Models.IStorage;
using AppData.Infrastructures.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];


// Configura l'autenticazione con JWT Bearer
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        //Verifica che il token non sia scaduto.
//        ValidateLifetime = true,
//        // Verifica la chiave di firma del token
//        ValidateIssuerSigningKey = true,
//        //Verifica che il token sia emesso da un'autorità valida.
//        ValidIssuer = jwtSettings["Issuer"],
//        // Se stai sviluppando un'API denominata "MyAPI", potresti impostare ValidAudience su "MyAPI".
//        ValidAudience = jwtSettings["Audience"],
//        // Questa chiave è utilizzata per firmare e validare i token JWT, garantendo l'integrità e l'autenticità del token
//        // Puoi generare una chiave sicura utilizzando un generatore di stringhe casuali o strumenti specifici per la generazione di chiavi.
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
//    };
//});

builder.Services.AddSingleton<TokenProvider>();


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
builder.Services.AddScoped<IUserService, UserService>();


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
        // Gestione degli errori
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        await AdminInitializer.Initialize(services, userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Errore durante l'inizializzazione.");
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
