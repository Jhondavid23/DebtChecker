using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DebtCheckerBackend.BLL.Contrato;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using DebtCheckerBackend.DTO;

namespace DebtCheckerBackend.BLL
{
    public class DynamoDbCacheService : ICacheService
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly ILogger<DynamoDbCacheService> _logger;
        private readonly string _tableName;

        public DynamoDbCacheService(
            IAmazonDynamoDB dynamoDb,
            ILogger<DynamoDbCacheService> logger,
            IConfiguration configuration)
        {
            _dynamoDb = dynamoDb;
            _logger = logger;
            _tableName = configuration.GetValue<string>("DynamoDb:CacheTableName") ?? "DebtCache";
        }

        /// <summary>
        /// Obtiene datos del caché por clave
        /// </summary>
        /// <typeparam name="T">Tipo de datos a obtener</typeparam>
        /// <param name="cacheKey">Clave única del caché</param>
        /// <returns>Datos deserializados o null si no existen/expiraron</returns>
        public async Task<T?> GetAsync<T>(string cacheKey) where T : class
        {
            try
            {
                _logger.LogDebug("Intentando obtener del caché: {CacheKey}", cacheKey);

                var request = new GetItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "CacheKey", new AttributeValue { S = cacheKey } }
                    }
                };

                var response = await _dynamoDb.GetItemAsync(request);

                // Si no se encontró el item
                if (!response.Item.Any())
                {
                    _logger.LogDebug("Caché miss para clave: {CacheKey}", cacheKey);
                    return null;
                }

                // Verificar si los datos han expirado
                if (response.Item.ContainsKey("ExpiresAt"))
                {
                    if (DateTime.TryParse(response.Item["ExpiresAt"].S, out var expiresAt))
                    {
                        if (DateTime.UtcNow > expiresAt)
                        {
                            _logger.LogDebug("Datos expirados en caché para clave: {CacheKey}", cacheKey);

                            // Eliminar datos expirados de manera asíncrona (fire and forget)
                            _ = Task.Run(() => DeleteAsync(cacheKey));
                            return null;
                        }
                    }
                }

                // Si llegamos aquí, tenemos datos válidos
                if (!response.Item.ContainsKey("Data"))
                {
                    _logger.LogWarning("Item de caché sin datos para clave: {CacheKey}", cacheKey);
                    return null;
                }

                var jsonData = response.Item["Data"].S;
                var deserializedData = JsonSerializer.Deserialize<T>(jsonData);

                _logger.LogDebug("Caché hit para clave: {CacheKey}", cacheKey);
                return deserializedData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener del caché: {CacheKey}", cacheKey);
                // En caso de error, devolver null para que la aplicación consulte la fuente original
                return null;
            }
        }

        /// <summary>
        /// Guarda datos en el caché
        /// </summary>
        /// <typeparam name="T">Tipo de datos a guardar</typeparam>
        /// <param name="cacheKey">Clave única del caché</param>
        /// <param name="data">Datos a guardar</param>
        /// <param name="expiration">Tiempo de expiración opcional</param>
        public async Task SetAsync<T>(string cacheKey, T data, TimeSpan? expiration = null) where T : class
        {
            try
            {
                if (data == null)
                {
                    _logger.LogWarning("Intentando guardar datos null en caché para clave: {CacheKey}", cacheKey);
                    return;
                }

                _logger.LogDebug("Guardando en caché: {CacheKey}", cacheKey);

                var item = new Dictionary<string, AttributeValue>
                {
                    { "CacheKey", new AttributeValue { S = cacheKey } },
                    { "Data", new AttributeValue { S = JsonSerializer.Serialize(data) } },
                    { "CreatedAt", new AttributeValue { S = DateTime.UtcNow.ToString("O") } },
                    { "UpdatedAt", new AttributeValue { S = DateTime.UtcNow.ToString("O") } }
                };

                // Agregar tiempo de expiración si se especifica
                if (expiration.HasValue)
                {
                    var expiresAt = DateTime.UtcNow.Add(expiration.Value);
                    item["ExpiresAt"] = new AttributeValue { S = expiresAt.ToString("O") };

                    // También agregar TTL para DynamoDB 
                    // item["TTL"] = new AttributeValue { N = ((DateTimeOffset)expiresAt).ToUnixTimeSeconds().ToString() };
                }

                var request = new PutItemRequest
                {
                    TableName = _tableName,
                    Item = item
                };

                await _dynamoDb.PutItemAsync(request);

                _logger.LogDebug("Datos guardados exitosamente en caché: {CacheKey}", cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar en caché: {CacheKey}", cacheKey);
                // No lanzar excepción para que la aplicación continúe funcionando sin caché
            }
        }

        /// <summary>
        /// Elimina datos del caché
        /// </summary>
        /// <param name="cacheKey">Clave del caché a eliminar</param>
        public async Task DeleteAsync(string cacheKey)
        {
            try
            {
                _logger.LogDebug("Eliminando del caché: {CacheKey}", cacheKey);

                var request = new DeleteItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "CacheKey", new AttributeValue { S = cacheKey } }
                    }
                };

                await _dynamoDb.DeleteItemAsync(request);

                _logger.LogDebug("Datos eliminados exitosamente del caché: {CacheKey}", cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar del caché: {CacheKey}", cacheKey);
                // No lanzar excepción
            }
        }

        /// <summary>
        /// Verifica si existe una clave en el caché
        /// </summary>
        /// <param name="cacheKey">Clave a verificar</param>
        /// <returns>True si existe y no ha expirado, False en caso contrario</returns>
        public async Task<bool> ExistsAsync(string cacheKey)
        {
            try
            {
                var data = await GetAsync<object>(cacheKey);
                return data != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia en caché: {CacheKey}", cacheKey);
                return false;
            }
        }

        /// <summary>
        /// Limpia entradas expiradas del caché (método de mantenimiento)
        /// </summary>
        /// <returns>Número de entradas eliminadas</returns>
        public async Task<int> CleanupExpiredEntriesAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando limpieza de entradas expiradas del caché");

                var scanRequest = new ScanRequest
                {
                    TableName = _tableName,
                    FilterExpression = "ExpiresAt < :currentTime",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":currentTime", new AttributeValue { S = DateTime.UtcNow.ToString("O") } }
                    }
                };

                var scanResponse = await _dynamoDb.ScanAsync(scanRequest);
                var expiredItems = scanResponse.Items;

                int deletedCount = 0;
                foreach (var item in expiredItems)
                {
                    try
                    {
                        var deleteRequest = new DeleteItemRequest
                        {
                            TableName = _tableName,
                            Key = new Dictionary<string, AttributeValue>
                            {
                                { "CacheKey", item["CacheKey"] }
                            }
                        };

                        await _dynamoDb.DeleteItemAsync(deleteRequest);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error al eliminar entrada expirada: {CacheKey}",
                            item["CacheKey"].S);
                    }
                }

                _logger.LogInformation("Limpieza de caché completada. Entradas eliminadas: {DeletedCount}", deletedCount);
                return deletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la limpieza del caché");
                return 0;
            }
        }

        /// <summary>
        /// Obtiene estadísticas del caché para monitoreo
        /// </summary>
        /// <returns>Objeto con estadísticas básicas</returns>
        public async Task<CacheStatistics> GetStatisticsAsync()
        {
            try
            {
                var scanRequest = new ScanRequest
                {
                    TableName = _tableName,
                    Select = Select.COUNT
                };

                var scanResponse = await _dynamoDb.ScanAsync(scanRequest);

                return new CacheStatistics
                {
                    TotalEntries = scanResponse.Count ?? 0,
                    TableName = _tableName,
                    LastUpdated = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas del caché");
                return new CacheStatistics
                {
                    TotalEntries = -1,
                    TableName = _tableName,
                    LastUpdated = DateTime.UtcNow,
                    Error = ex.Message
                };
            }
        }

        /// <summary>
        /// Inicializa la tabla de caché si no existe
        /// </summary>
        public async Task<bool> InitializeCacheTableAsync()
        {
            try
            {
                _logger.LogInformation("Verificando si existe la tabla de caché: {TableName}", _tableName);

                // Verificar si la tabla ya existe
                var listTablesRequest = new ListTablesRequest();
                var listTablesResponse = await _dynamoDb.ListTablesAsync(listTablesRequest);

                if (listTablesResponse.TableNames.Contains(_tableName))
                {
                    _logger.LogInformation("La tabla de caché ya existe: {TableName}", _tableName);
                    return true;
                }

                // Crear la tabla si no existe
                _logger.LogInformation("Creando tabla de caché: {TableName}", _tableName);

                var createTableRequest = new CreateTableRequest
                {
                    TableName = _tableName,
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "CacheKey",
                            KeyType = KeyType.HASH
                        }
                    },
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "CacheKey",
                            AttributeType = ScalarAttributeType.S
                        }
                    },
                    BillingMode = BillingMode.PAY_PER_REQUEST
                };

                var createTableResponse = await _dynamoDb.CreateTableAsync(createTableRequest);

                // Esperar hasta que la tabla esté activa
                var tableStatus = TableStatus.CREATING;
                while (tableStatus == TableStatus.CREATING)
                {
                    await Task.Delay(5000); // Esperar 5 segundos

                    var describeRequest = new DescribeTableRequest { TableName = _tableName };
                    var describeResponse = await _dynamoDb.DescribeTableAsync(describeRequest);
                    tableStatus = describeResponse.Table.TableStatus;
                }

                _logger.LogInformation("Tabla de caché creada exitosamente: {TableName}", _tableName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inicializar tabla de caché: {TableName}", _tableName);
                return false;
            }
        }
    }

    
}