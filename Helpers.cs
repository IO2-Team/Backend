using dionizos_backend_app.Models;
using Microsoft.AspNetCore.Mvc;

namespace dionizos_backend_app
{

    public interface IHelper
    {
        public bool Validate(string sessionToken, TimeSpan sessionLength);
    }
    public class Helpers : IHelper
    {
        private DionizosDataContext _dionizosDataContext;
        public Helpers(DionizosDataContext dionizosDataContext)
        {
            _dionizosDataContext = dionizosDataContext;
        }
        public bool Validate(string sessionToken, TimeSpan sessionLength)
        {
            return _dionizosDataContext.Sessions.Count(x => x.Token == sessionToken && x.Time.ToUniversalTime() >= (DateTime.UtcNow - sessionLength)) > 0;

        }

    }
}
