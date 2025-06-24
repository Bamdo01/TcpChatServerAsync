using System.Security.Cryptography;
using System.Text;

public static class SecurityHelper
{
    public static string GenerateSalt(int size = 16)
    {
        byte[] saltBytes = new byte[size];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    public static string HashWithSalt(string passwordHashFromClient, string salt)
    {
        //빌드패턴 
        using (SHA256 sha = SHA256.Create())
        {
            string combined = passwordHashFromClient + salt;
            byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
            return Convert.ToBase64String(hashBytes);
        }
    }
}