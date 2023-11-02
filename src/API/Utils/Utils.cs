using System.Security.Cryptography;

namespace API.Utils
{
    public class Utils
    {
        public const string CONTENT_ROOT_PLACE_HOLDER = "%CONTENTROOTPATH%";
        public static string GetDbConnectionString(IConfiguration configuration, string contentRootPath)
        {
            contentRootPath = contentRootPath.Replace("API", "Data");
            var connectionString = configuration["ConnectionString"];

            if (connectionString is not null && connectionString.Contains(CONTENT_ROOT_PLACE_HOLDER))
            {
                connectionString = connectionString.Replace(CONTENT_ROOT_PLACE_HOLDER, contentRootPath);
            }
            configuration["ConnectionString"] = connectionString;
            return connectionString;
        }

        public static string Generate256BitKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] keyBytes = new byte[32];
                rng.GetBytes(keyBytes);
                return Convert.ToBase64String(keyBytes);
            }
        }
    }
}
