using dionizos_backend_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dionizos_backend_app
{

    public interface IHelper
    {
        public Organizer? Validate(string sessionToken);
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
        private Organizer? Validate(string sessionToken, TimeSpan sessionLength)
        {
            var lastSession = _dionizosDataContext.Sessions
                                                  .Include(x => x.Organizer)
                                                  .FirstOrDefault(x => x.Token == sessionToken);
            if (lastSession is null) return null;
            var mostRecentSession = _dionizosDataContext.Sessions
                .Where( x=> x.Organizer == lastSession.Organizer)
                .Include(x => x.Organizer)
                .ThenInclude(x => x.Events)
                .OrderBy(x => x.Time)
                .LastOrDefault();

            if (mostRecentSession.Id != lastSession.Id)
            {
                return null;
            }
            if (mostRecentSession.Time.ToUniversalTime() < (DateTime.UtcNow - sessionLength))
            {
                return null;
            }
            return mostRecentSession.Organizer;
        }
        public Organizer? Validate(string sessionToken)
        {
            return Validate(sessionToken, TimeSpan.FromSeconds(GetSessionLengthSeconds()));
        }

        private int GetSessionLengthSeconds()
        {
            return int.Parse(_configurationRoot["SessionLengthSeconds"]);
        }

    }
}
