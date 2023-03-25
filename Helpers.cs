using dionizos_backend_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dionizos_backend_app
{
    public interface IHelper
    {
        public Task<bool> Validate(string sessionToken, TimeSpan sessionLength);
        public int GetSessionLengthHours();
    }
    public class Helpers : IHelper
    {
        private readonly DionizosDataContext _dionizosDataContext;
        private readonly IConfigurationRoot _configurationRoot;
        public Helpers(DionizosDataContext dionizosDataContext, IConfigurationRoot configurationRoot)
        {
            _dionizosDataContext = dionizosDataContext;
            _configurationRoot = configurationRoot;
        }
        public async Task<bool> Validate(string sessionToken, TimeSpan sessionLength)
        {
            return await _dionizosDataContext.Sessions.AnyAsync(x => x.Token == sessionToken && x.Time.ToUniversalTime() >= (DateTime.UtcNow - sessionLength));
        }

        public int GetSessionLengthHours()
        {
            return int.Parse(_configurationRoot["SessionLengthHours"]);
        }
    }
}