using DebtCheckerBackend.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtCheckerBackend.BLL.Contrato
{
    public interface ICacheService
    {
        /// <summary>
        /// Obtiene datos del caché por clave
        /// </summary>
        /// <typeparam name="T">Tipo de datos a obtener</typeparam>
        /// <param name="cacheKey">Clave única del caché</param>
        /// <returns>Datos deserializados o null si no existen/expiraron</returns>
        Task<T?> GetAsync<T>(string cacheKey) where T : class;

        /// <summary>
        /// Guarda datos en el caché
        /// </summary>
        /// <typeparam name="T">Tipo de datos a guardar</typeparam>
        /// <param name="cacheKey">Clave única del caché</param>
        /// <param name="data">Datos a guardar</param>
        /// <param name="expiration">Tiempo de expiración opcional</param>
        Task SetAsync<T>(string cacheKey, T data, TimeSpan? expiration = null) where T : class;

        /// <summary>
        /// Elimina datos del caché
        /// </summary>
        /// <param name="cacheKey">Clave del caché a eliminar</param>
        Task DeleteAsync(string cacheKey);

        /// <summary>
        /// Verifica si existe una clave en el caché
        /// </summary>
        /// <param name="cacheKey">Clave a verificar</param>
        /// <returns>True si existe y no ha expirado, False en caso contrario</returns>
        Task<bool> ExistsAsync(string cacheKey);

        /// <summary>
        /// Limpia entradas expiradas del caché (método de mantenimiento)
        /// </summary>
        /// <returns>Número de entradas eliminadas</returns>
        Task<int> CleanupExpiredEntriesAsync();

        /// <summary>
        /// Obtiene estadísticas del caché para monitoreo
        /// </summary>
        /// <returns>Objeto con estadísticas básicas</returns>
        Task<CacheStatistics> GetStatisticsAsync();

        /// <summary>
        /// Inicializa la tabla de caché si no existe
        /// </summary>
        Task<bool> InitializeCacheTableAsync();
    }
}