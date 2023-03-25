using dionizos_backend_app.Extensions;

namespace dionizos_backend_app.Models
{
    public partial class Session
    {
        public static string GetToken(string email, DateTime time)
        {
            string result = (email + time.ToString()).Base64Encode();
            return result.Length <= 255
                ? result
                : result[..255];
        }
    }
}
