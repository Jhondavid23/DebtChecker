using DebtCheckerBackend.BLL.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebtCheckerBackend.BLL
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            // BCrypt genera automáticamente el salt y es más seguro que SHA
            return BCrypt.Net.BCrypt.HashPassword(password, 12); // 12 rounds para buena seguridad
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
