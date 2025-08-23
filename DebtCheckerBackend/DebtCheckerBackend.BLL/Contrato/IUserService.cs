using DebtCheckerBackend.DTO;
using DebtCheckerBackend.Model;
using DebtCheckerBackend.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtCheckerBackend.BLL.Contrato
{
    public interface IUserService
    {
        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        /// <param name="request">Datos de registro</param>
        /// <returns>Resultado con UserDto si es exitoso</returns>
        Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Obtiene un usuario por ID (devuelve DTO sin contraseña)
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>UserDto o null si no existe</returns>
        Task<UserDto?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un usuario por email (devuelve DTO sin contraseña)
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <returns>UserDto o null si no existe</returns>
        Task<UserDto?> GetByEmailAsync(string email);

        /// <summary>
        /// Obtiene la entidad User completa por email (incluye password hash)
        /// SOLO para autenticación interna - NO exponer en controladores
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <returns>User entity o null si no existe</returns>
        Task<User?> GetUserEntityByEmailAsync(string email);

        /// <summary>
        /// Obtiene la entidad User completa por ID (incluye password hash)
        /// SOLO para operaciones internas - NO exponer en controladores
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>User entity o null si no existe</returns>
        Task<User?> GetUserEntityByIdAsync(int id);

        /// <summary>
        /// Verifica si existe un usuario con el email dado
        /// </summary>
        /// <param name="email">Email a verificar</param>
        /// <returns>True si existe, False si no</returns>
        Task<bool> ExistsByEmailAsync(string email);

        /// <summary>
        /// Actualiza la información básica del usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="updateRequest">Datos a actualizar</param>
        /// <returns>UserDto actualizado</returns>
        Task<ApiResponse<UserDto>> UpdateUserAsync(int userId, UpdateUserRequest updateRequest);

        /// <summary>
        /// Cambia la contraseña del usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="currentPassword">Contraseña actual</param>
        /// <param name="newPassword">Nueva contraseña</param>
        /// <returns>Resultado de la operación</returns>
        Task<ApiResponse<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}