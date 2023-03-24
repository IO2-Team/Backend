using dionizos_backend_app.Models;
using Microsoft.AspNetCore.Mvc;

namespace dionizos_backend_app
{

    public interface IHelper
    {
        public bool Validate(string sessionToken, TimeSpan sessionLength);
        public int GetSessionLengthHours();
    }
    public class Helpers : IHelper
    {
        private DionizosDataContext _dionizosDataContext;
        private IConfigurationRoot _configurationRoot;
        public Helpers(DionizosDataContext dionizosDataContext, IConfigurationRoot configurationRoot)
        {
            _dionizosDataContext = dionizosDataContext;
            _configurationRoot = configurationRoot;
        }
        public bool Validate(string sessionToken, TimeSpan sessionLength)
        {
            return _dionizosDataContext.Sessions.Count(x => x.Token == sessionToken && x.Time.ToUniversalTime() >= (DateTime.UtcNow - sessionLength)) > 0;

        }

        public int GetSessionLengthHours()
        {
            return int.Parse(_configurationRoot["SessionLengthHours"]);
        }
    }
}
