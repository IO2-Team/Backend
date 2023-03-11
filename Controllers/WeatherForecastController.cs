using dionizos_backend_app.Models;
using Microsoft.AspNetCore.Mvc;
namespace dionizos_backend_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly DionizosDataContext _DionizosDataContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, DionizosDataContext dionizosDataContext)
        {
            _logger = logger;
            _DionizosDataContext = dionizosDataContext;
        }

        [HttpGet]
        [Route("test")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet]
        [Route("organizer/{id}")]
        public IActionResult GetOrganizer(int id)
        {
            var organizer = _DionizosDataContext.Organizers.Find(id);
            if (organizer == null)
            {
                return NotFound();
            }
            return Ok(organizer);
        }
    }
}