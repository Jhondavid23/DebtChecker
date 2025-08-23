using DebtCheckerBackend.DAL.Repositorios.Contrato;
using DebtCheckerBackend.DAL.Repositorios;
using DebtCheckerBackend.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebtCheckerBackend.BLL.Contrato;
using DebtCheckerBackend.BLL;

namespace DebtCheckerBackend.IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencias(this IServiceCollection services, IConfiguration configuration)
        {
            // Local en appsettings.json
            // var connectionString = configuration.GetConnectionString("DebtManagementAppContext");

            // Variable de entorno
            var connectionString = Environment.GetEnvironmentVariable("DEBT_MANAGEMENT_DB_CONNECTION_STRING") ?? configuration.GetConnectionString("DebtManagementAppContext");

            // Inyectar el DbContext con la cadena de conexión
            services.AddDbContext<DebtManagementAppContext>(options => options.UseNpgsql(connectionString));

            // Inyectar repositorios
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Services
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICacheService, DynamoDbCacheService>();
            services.AddScoped<IDebtService, DebtService>();
        }
    }
}
