using dionizos_backend_app.Extensions;
using dionizos_backend_app.Models;
using Org.OpenAPITools.Models;
using Quartz;

namespace dionizos_backend_app
{
    public class RefreshEventStatus : IJob
    {
        DionizosDataContext _dionizosDataContext;
        IHelper _helper;
        ILogger _logger;

        public RefreshEventStatus(DionizosDataContext dionizosDataContext, IHelper helper, ILogger<RefreshEventStatus> logger)
        {
            _dionizosDataContext = dionizosDataContext;
            _helper = helper;
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.Log(LogLevel.Information, "Cron job: RefreshEventStatus");

            var events = _dionizosDataContext.Events.Where(e => e.Status != (int)EventStatus.CancelledEnum && e.Status != (int)EventStatus.DoneEnum);
            foreach (var e in events)
            {
                bool wasChanged = false;
                if(e.Status == (int)EventStatus.InFutureEnum && e.Starttime < DateTime.UtcNow)
                {
                    e.Status = (int)EventStatus.PendingEnum;
                    wasChanged = true;
                }
                if (e.Status == (int)EventStatus.PendingEnum && e.Endtime < DateTime.UtcNow)
                {
                    e.Status = (int)EventStatus.DoneEnum;
                    wasChanged = true;
                }
                if(wasChanged)
                {
                    _logger.Log(LogLevel.Information, $"Update event status: id: {e.Id.ToString()}, status: {((EventStatus)e.Status).toStringEnum()}");
                    _dionizosDataContext.Update(e);
                }
            }
            _dionizosDataContext.SaveChanges();
            _logger.Log(LogLevel.Information, "Cron job: Done");
            return Task.FromResult(true);
        }
    }
}