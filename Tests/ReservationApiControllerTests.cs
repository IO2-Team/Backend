using System;
using System.Linq;
using System.Threading.Tasks;
using dionizos_backend_app;
using dionizos_backend_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Org.OpenAPITools.Controllers;
using Org.OpenAPITools.Models;
using Xunit;

namespace Tests
{
    public class ReservationApiControllerTests
    {
        private readonly DionizosDataContext _context;
        private readonly ReservationApiController _controller;
        private readonly IHelper _helper;
        private readonly Mock<IConfigurationRoot> _mockConfig;

        public ReservationApiControllerTests()
        {
            _mockConfig = new Mock<IConfigurationRoot>();
            var options = new DbContextOptionsBuilder<DionizosDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DionizosDataContext(options);

            _helper = new dionizos_backend_app.Helpers(_context, _mockConfig.Object);
            _controller = new ReservationApiController(_context, _helper);
        }

        [Fact]
        public async Task MakeReservation_ReturnsStatusCode201_WhenReservationIsCreated()
        {
            // Arrange
            Event testEvent = new Event
            {
                Id = 1,
                Owner = 1,
                Title = "Test Event",
                Starttime = DateTime.Now + TimeSpan.FromHours(1),
                Endtime = DateTime.Now + TimeSpan.FromHours(2),
                Latitude = "12.34",
                Longitude = "56.78",
                Status = 1,
                Placecapacity = 10
            };
            _context.Events.Add(testEvent);
            _context.SaveChanges();

            // Act
            var result = await _controller.MakeReservation(testEvent.Id, null);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, ((ObjectResult)result).StatusCode);
            Assert.Single(_context.Reservatons);
        }

        [Fact]
        public async Task MakeReservation_ReturnsStatusCode400_WhenNoFreePlace()
        {
            // Arrange
            Event testEvent = new Event
            {
                Id = 1,
                Owner = 1,
                Title = "Test Event",
                Starttime = DateTime.Now + TimeSpan.FromHours(1),
                Endtime = DateTime.Now + TimeSpan.FromHours(2),
                Latitude = "12.34",
                Longitude = "56.78",
                Status = 1,
                Placecapacity = 1
            };
            _context.Events.Add(testEvent);
            _context.Reservatons.Add(new Reservaton { EventId = testEvent.Id, PlaceId = 0, Token = "testToken" });
            _context.SaveChanges();

            // Act
            var result = await _controller.MakeReservation(testEvent.Id, null);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(400, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task MakeReservation_ReturnsStatusCode404_WhenEventDoesNotExistOrIsDone()
        {
            // Arrange
            long nonExistingEventId = 999;

            // Act
            var result = await _controller.MakeReservation(nonExistingEventId, null);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task DeleteReservation_ReturnsStatusCode204_WhenReservationIsDeleted()
        {
            // Arrange
            Event testEvent = new Event
            {
                Id = 1,
                Owner = 1,
                Title = "Test Event",
                Starttime = DateTime.Now + TimeSpan.FromHours(1),
                Endtime = DateTime.Now + TimeSpan.FromHours(2),
                Latitude = "12.34",
                Longitude = "56.78",
                Status = 1,
                Placecapacity = 10
            };
            _context.Events.Add(testEvent);
            _context.Reservatons.Add(new Reservaton { EventId = testEvent.Id, PlaceId = 0, Token = "testToken" });
            _context.SaveChanges();

            // Act
            var result = await _controller.DeleteReservation("testToken");

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(204, ((StatusCodeResult)result).StatusCode);
            Assert.Empty(_context.Reservatons);
        }

        [Fact]
        public async Task DeleteReservation_ReturnsStatusCode404_WhenReservationTokenNotFound()
        {
            // Arrange
            string nonExistingToken = "nonExistingToken";

            // Act
            var result = await _controller.DeleteReservation(nonExistingToken);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, ((StatusCodeResult)result).StatusCode);
        }
    }
}