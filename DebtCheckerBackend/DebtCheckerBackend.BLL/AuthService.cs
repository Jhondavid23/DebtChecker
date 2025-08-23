using DebtCheckerBackend.BLL.Contrato;
using DebtCheckerBackend.DAL.Repositorios.Contrato;
using DebtCheckerBackend.DTO;
using DebtCheckerBackend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtCheckerBackend.BLL
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public AuthService(
            IGenericRepository<User> userRepository,
            IUserService userService,
            IPasswordService passwordService,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _userService = userService;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                // Buscar usuario por email 
                var user = await _userRepository.Obtener(u => u.Email == request.Email.ToLower().Trim());

                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Credenciales inválidas",
                        Errors = new List<string> { "Email o contraseña incorrectos" }
                    };
                }

                // Verificar contraseña 
                if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Credenciales inválidas",
                        Errors = new List<string> { "Email o contraseña incorrectos" }
                    };
                }

                // Generar token JWT
                var token = _jwtService.GenerateToken(user);
                var expiration = _jwtService.GetTokenExpiration();

                // Convertir usuario a DTO (SIN PASSWORD)
                var userDto = MapToUserDto(user);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Login exitoso",
                    Token = token,
                    ExpiresAt = expiration,
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var registrationResult = await _userService.RegisterAsync(request);

                if (!registrationResult.Success)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = registrationResult.Message,
                        Errors = registrationResult.Errors
                    };
                }

                // Si el registro fue exitoso, podemos generar un token automáticamente
                // para que el usuario no tenga que hacer login después del registro
                if (registrationResult.Data != null)
                {
                    // Buscar el usuario completo para generar el token
                    var user = await _userRepository.Obtener(u => u.Email == request.Email.ToLower().Trim());

                    if (user != null)
                    {
                        var token = _jwtService.GenerateToken(user);
                        var expiration = _jwtService.GetTokenExpiration();

                        return new AuthResponse
                        {
                            Success = true,
                            Message = "Usuario registrado exitosamente",
                            Token = token,
                            ExpiresAt = expiration,
                            User = registrationResult.Data
                        };
                    }
                }

                return new AuthResponse
                {
                    Success = true,
                    Message = registrationResult.Message,
                    User = registrationResult.Data
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Error interno del servidor durante el registro",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<UserDto?> GetCurrentUserAsync(int userId)
        {
            try
            {
                return await _userService.GetByIdAsync(userId);
            }
            catch (Exception ex)
            {
                // Log the exception if you have a logger
                return null;
            }
        }

        /// <summary>
        /// Método helper para convertir User entity a UserDto
        /// </summary>
        /// <param name="user">Entidad User</param>
        /// <returns>DTO sin información sensible</returns>
        private static UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow
            };
        }

        /// <summary>
        /// Valida las credenciales sin devolver información sensible
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>UserDto si las credenciales son válidas, null en caso contrario</returns>
        public async Task<UserDto?> ValidateCredentialsAsync(string email, string password)
        {
            try
            {
                var user = await _userRepository.Obtener(u => u.Email == email.ToLower().Trim());

                if (user != null && _passwordService.VerifyPassword(password, user.PasswordHash))
                {
                    return MapToUserDto(user);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}