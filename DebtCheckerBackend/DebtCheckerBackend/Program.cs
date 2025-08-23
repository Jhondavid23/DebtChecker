using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using DebtCheckerBackend.BLL.Contrato;
using DebtCheckerBackend.DTO;
using DebtCheckerBackend.IOC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURACIÓN BÁSICA
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CONFIGURACIÓN DE AWS DYNAMODB
var awsAccessKey = builder.Configuration["AWS:AccessKey"];
var awsSecretKey = builder.Configuration["AWS:SecretKey"];
var awsRegion = builder.Configuration["AWS:Region"] ?? "us-east-1";

// Log para verificar configuración
var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger<Program>();
logger.LogInformation("AWS Access Key configurado: {HasKey}", !string.IsNullOrEmpty(awsAccessKey));
logger.LogInformation("AWS Secret Key configurado: {HasSecret}", !string.IsNullOrEmpty(awsSecretKey));
logger.LogInformation("AWS Region: {Region}", awsRegion);

// Validar credenciales
if (string.IsNullOrEmpty(awsAccessKey) || string.IsNullOrEmpty(awsSecretKey))
{
    throw new InvalidOperationException(
        $"AWS credentials are not configured");
}

// CONFIGURACIÓN ÚNICA DE DYNAMODB (eliminar AddAWSService)
builder.Services.AddSingleton<IAmazonDynamoDB>(provider =>
{
    var serviceLogger = provider.GetRequiredService<ILogger<IAmazonDynamoDB>>();

    try
    {
        serviceLogger.LogInformation("Configurando cliente DynamoDB...");

        var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
        var config = new AmazonDynamoDBConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(awsRegion),
            UseHttp = false, // HTTPS
            MaxErrorRetry = 3,
            Timeout = TimeSpan.FromSeconds(30)
        };

        var client = new AmazonDynamoDBClient(credentials, config);
        serviceLogger.LogInformation("Cliente DynamoDB configurado correctamente");

        return client;
    }
    catch (Exception ex)
    {
        serviceLogger.LogError(ex, "Error al configurar cliente DynamoDB: {Error}", ex.Message);
        throw;
    }
});

// CONFIGURACIÓN DE JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
{
    throw new InvalidOperationException("JWT settings are not configured properly in appsettings.json");
}

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// CONFIGURACIÓN DE CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "https://localhost:3000", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// INYECTAR DEPENDENCIAS PERSONALIZADAS
builder.Services.InyectarDependencias(builder.Configuration);

// CONSTRUCCIÓN DE LA APLICACIÓN
var app = builder.Build();

// INICIALIZACIÓN DE DYNAMODB
using (var scope = app.Services.CreateScope())
{
    var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
    var appLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        appLogger.LogInformation("Inicializando tabla de caché DynamoDB...");
        var initialized = await cacheService.InitializeCacheTableAsync();

        if (initialized)
        {
            appLogger.LogInformation("Tabla de caché inicializada correctamente");

            // Probar conexión guardando un valor de prueba
            await cacheService.SetAsync("startup_test", "API iniciada correctamente", TimeSpan.FromMinutes(1));
            appLogger.LogInformation("Prueba de caché exitosa");
        }
        else
        {
            appLogger.LogWarning("No se pudo inicializar la tabla de caché");
        }
    }
    catch (Exception ex)
    {
        appLogger.LogError(ex, "Error al inicializar la tabla de caché: {Error}", ex.Message);
        appLogger.LogWarning("La aplicación continuará sin caché DynamoDB");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



app.Run();