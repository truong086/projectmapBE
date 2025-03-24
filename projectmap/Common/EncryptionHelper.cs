using System.Security.Cryptography;
using System.Text;

namespace projectmap.Common
{
    public enum HashedPasswordFormat
    {
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }
    public static class EncryptionHelper
    {

        // Hàm mã hóa mật khẩu
        public static string CreatePasswordHash(string password, string key, HashedPasswordFormat hashedPasswordFormat = HashedPasswordFormat.SHA256)
        {
            string ConverPasswordAndKey = string.Concat(password, key);
            HashAlgorithm hashAlgorithm = hashedPasswordFormat switch
            {
                HashedPasswordFormat.SHA512 => SHA256.Create(),
                HashedPasswordFormat.SHA384 => SHA384.Create(),
                HashedPasswordFormat.SHA256 => SHA256.Create(),
                HashedPasswordFormat.SHA1 => SHA1.Create(),
                _ => throw new NotSupportedException("Not supported format")
            };

            if (hashAlgorithm == null)
                throw new ArgumentException("Null");

            var mahoa = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(ConverPasswordAndKey));
            return BitConverter.ToString(mahoa).Replace("-", "");
        }
    }
}
