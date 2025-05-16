using AppData.API.Extensions;
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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppData.EmailSender.Extensions;
using AppData.EmailSender.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add("role", ClaimTypes.Role);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            //Verifica che il token sia emesso da un'autorit� valida.
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            // Questa chiave � utilizzata per firmare e validare i token JWT, garantendo l'integrit� e l'autenticit� del token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        };
    });


// Configura il contesto del database
builder.Services.AddDbContext<AppDataSqlServerContext>(options =>
options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.MigrationsAssembly("AppData.Migrations")));

builder.Services.Configure<SmtpConfiguration>(
    builder.Configuration.GetSection("SmtpConfiguration"));

builder.Services.AddEmailSenderService();

// Configura ASP.NET Core Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDataSqlServerContext>()
    .AddDefaultTokenProviders();

// Aggiungi i servizi dell'applicazione
builder.Services.AddSingleton<TokenProvider>();
builder.Services.AddScoped<IWordService, WordService>();
builder.Services.AddScoped<IWordStorage, WordStorage>();
builder.Services.AddScoped<IUserService, UserService>();



// Aggiungi i servizi per i controller e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();


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

app.UseRouting();

// Aggiungi autenticazione e autorizzazione
app.UseAuthentication();
app.UseAuthorization();

//app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
