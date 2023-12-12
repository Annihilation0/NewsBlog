using System.Security.Cryptography;
using System.Text;

namespace NewsBlog.NewsBlogData
{
    public class PasswordHashing
    {
        /*--------------------------------------
        Генерация хэш-а от входной строки (пароль пользователя)
        --------------------------------------*/
        private static byte[] GetHash(string inputString)
        {
            /*--------------------------------------
            Возвращает хэш в виде массива байтов byte[]
            --------------------------------------*/
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            /*--------------------------------------
            Возвращает строковое представление хэша
            --------------------------------------*/
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

    }
}
