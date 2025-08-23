using Amazon.DynamoDBv2;
using DebtCheckerBackend.BLL.Contrato;
using DebtCheckerBackend.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings?.Issuer,
            ValidAudience = jwtSettings?.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? "default-key")),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// Configurar AWS DynamoDB
builder.Services.AddAWSService<IAmazonDynamoDB>(options =>
{
    // Configurar región
    options.Region = RegionEndpoint.GetBySystemName(
        builder.Configuration.GetValue<string>("AWS:Region") ?? "us-east-1");
});

// Configurar CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // Puertos comunes de React
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


var app = builder.Build();

// Inicializar tabla de caché al startup
using (var scope = app.Services.CreateScope())
{
    var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var initialized = await cacheService.InitializeCacheTableAsync();
        if (initialized)
        {
            logger.LogInformation("Tabla de caché inicializada correctamente");
        }
        else
        {
            logger.LogWarning("No se pudo inicializar la tabla de caché");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al inicializar la tabla de caché");
    }
}

// Configure the HTTP request pipeline.
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
