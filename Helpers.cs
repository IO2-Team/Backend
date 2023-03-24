using dionizos_backend_app.Models;

namespace dionizos_backend_app
{
    public static class Helpers
    {
        public static bool isSessionValid(DionizosDataContext dionizosDataContext, string sessionToken, TimeSpan sessionLength)
        {
            return dionizosDataContext.Sessions.Any(x => x.Token == sessionToken && x.Time.ToUniversalTime() >= (DateTime.UtcNow - sessionLength));

        }
    }
}