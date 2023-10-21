using System.Security.Cryptography;
using System.Text;

namespace notesApp.API.Utils
{
    public class HashingAndSaltingClass
    {
        //const int keySize = 64;
        //const int iterations = 350000;
        //HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public static void GetHashedPassword(string providedPassword, out string hashedPassword, out string salt)
        {
            byte[] saltBytes = new byte[20];

            RandomNumberGenerator.Create().GetNonZeroBytes(saltBytes);

            salt = Convert.ToBase64String(saltBytes);

            using (Rfc2898DeriveBytes hasher = new(providedPassword, saltBytes, 10000))
            {
                hashedPassword = Convert.ToBase64String(hasher.GetBytes(20));
            }
        }

        public static void CheckHashedPassword(string providedPassword, string providedSalt, out string hashedPassword)
        {
            using (Rfc2898DeriveBytes hasher = new Rfc2898DeriveBytes(providedPassword, Convert.FromBase64String(providedSalt), 10000))
            {
                hashedPassword = Convert.ToBase64String(hasher.GetBytes(20));
            }
        }
    }
}
