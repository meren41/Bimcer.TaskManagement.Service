using System.Security.Cryptography;

namespace Bimcer.TaskManagement.Service.Core.Security;

public static class HashingHelper
{
    public static string CreateHash(string password, int saltSize = 16, int iter = 10000, int bytes = 32)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(saltSize);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iter, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(bytes);

        // Salt ve hash'i birleştir (salt + hash)
        var hashWithSalt = new byte[saltSize + bytes];
        Array.Copy(saltBytes, 0, hashWithSalt, 0, saltSize);
        Array.Copy(hash, 0, hashWithSalt, saltSize, bytes);

        return Convert.ToBase64String(hashWithSalt);
    }

    public static bool Verify(string password, string base64Hash, int saltSize = 16, int iter = 10000, int bytes = 32)
    {
        var hashWithSalt = Convert.FromBase64String(base64Hash);
        var saltBytes = new byte[saltSize];
        Array.Copy(hashWithSalt, 0, saltBytes, 0, saltSize);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iter, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(bytes);

        for (int i = 0; i < bytes; i++)
        {
            if (hashWithSalt[saltSize + i] != hash[i])
                return false;
        }
        return true;
    }
}
