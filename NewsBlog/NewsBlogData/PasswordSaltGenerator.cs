using System.Security.Cryptography;

namespace NewsBlog.NewsBlogData
{
    public static class PasswordSaltGenerator
    {
        public static string GenerateSalt(int size)
        {
            /*--------------------------------------
            Генерация соли для пароля
            --------------------------------------*/

            // Создаем буфер для хранения байтов соли
            byte[] saltBytes = new byte[size];

            // Используем криптографический генератор случайных чисел для заполнения буфера
            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(saltBytes);
            }

            // Возвращаем соль как строку в формате Base64
            return Convert.ToBase64String(saltBytes);
        }
    }
}
