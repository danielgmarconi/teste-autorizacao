using System.Security.Cryptography;
using System.Text;

namespace TesteAutorizacao.Util
{
    public class CriptografiaFuncao
    {
        public static string GerarChavePublica() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
        private static byte[] CriarAesChave(string inputString) => Encoding.UTF8.GetByteCount(inputString) == 32 ?
                                                                  Encoding.UTF8.GetBytes(inputString) :
                                                                  SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));
        public static string Decriptar(string valor, string chavePublica, string chavePrivada)
        {
            if (valor is not { Length: > 0 })
                throw new ArgumentNullException(nameof(valor));
            if (chavePrivada is not { Length: > 0 })
                throw new ArgumentNullException(nameof(chavePrivada));
            if (chavePublica is not { Length: > 0 })
                throw new ArgumentNullException(nameof(chavePublica));
            using var aesAlg = Aes.Create();
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Key = CriarAesChave(chavePrivada);
            aesAlg.IV = Convert.FromBase64String(chavePublica);
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using var msDecrypt = new MemoryStream(Convert.FromBase64String(valor));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            var plaintext = srDecrypt.ReadToEnd();
            return plaintext;
        }
        public static string Encriptar(string valor, string chavePublica, string chavePrivada)
        {
            if (valor is not { Length: > 0 })
                throw new ArgumentNullException(nameof(valor));
            if (chavePrivada is not { Length: > 0 })
                throw new ArgumentNullException(nameof(chavePrivada));
            if (chavePublica is not { Length: > 0 })
                throw new ArgumentNullException(nameof(chavePublica));
            byte[] encrypted;
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Key = CriarAesChave(chavePrivada);
                aesAlg.IV = Convert.FromBase64String(chavePublica);
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(valor);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }
    }
}
