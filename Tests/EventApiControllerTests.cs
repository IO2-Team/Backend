using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dionizos_backend_app;
using dionizos_backend_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.OpenAPITools.Controllers;
using Org.OpenAPITools.Models;
using Xunit;
using Moq;

namespace Tests
{
    public class EventApiControllerTests
    {
    private readonly DionizosDataContext _context;
    private readonly EventApiController _controller;
    private readonly Mock<IHelper> _mockHelper;
    private readonly Mock<ILogger<EventApiController>> _mockLogger;

        public EventApiControllerTests()
    {
        _mockHelper = new Mock<IHelper>();
        _mockLogger = new Mock<ILogger<EventApiController>>();
        _mockHelper.Setup( x => x.Validate(It.IsAny<string>()))
            .Returns(( string s) =>
                {
                    if (_context.Sessions.Where(x => x.Token == s).Count() != 0)
                    {
                        return _context.Sessions.Where(x => x.Token == s).First().Organizer;
                    }
                    else
                    {
                        return null;
                    }
                }
            );

        var options = new DbContextOptionsBuilder<DionizosDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DionizosDataContext(options);

        _controller = new EventApiController(_context, _mockHelper.Object, _mockLogger.Object);
    }

        [Fact]
        public async Task AddEvent_ReturnsStatusCode201_WhenEventIsCreated()
        {
            // Arrange
            string sessionToken = "validSessionToken";
            EventFormDTO eventForm = new EventFormDTO
            {
                Title = "Test Event",
                StartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600,
                EndTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 7200,
                Latitude = "12.345678",
                Longitude = "98.765432",
                MaxPlace = 100,
                CategoriesIds = new List<int?> { 1 }
            };
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = 1
            };
            Session session = new Session
            {
                Id = 1,
                Organizer = organizer,
                OrganizerId = 1,
                Time = DateTime.Now - TimeSpan.FromMinutes(10),
                Token = "validSessionToken"
            };
            Category category = new Category
            {
                Id = 1,
                Name = "TestCategory"
            };
            _context.Add(organizer);
            _context.Add(session);
            _context.Add(category);
            _context.SaveChanges();

            // Act
            var result = await _controller.AddEvent(sessionToken, eventForm);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, ((ObjectResult)result).StatusCode);
            Assert.Single(_context.Events);
        }

        // Add more tests for different scenarios in AddEvent method

        [Fact]
        public async Task CancelEvent_ReturnsStatusCode204_WhenEventIsCancelled()
        {
            // Arrange
            string sessionToken = "validSessionToken";
            string eventId = "1";
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = 1
            };
            Session session = new Session
            {
                Id = 1,
                Organizer = organizer,
                OrganizerId = 1,
                Time = DateTime.Now - TimeSpan.FromMinutes(10),
                Token = "validSessionToken"
            };
            Event eventToCancel = new Event
            {
                Id = 1,
                Owner = 1,
                Title = "Test Event",
                Starttime = DateTime.UtcNow.AddHours(1),
                Endtime = DateTime.UtcNow.AddHours(2),
                Latitude = "12.345678",
                Longitude = "98.765432",
                Status = (int)EventStatus.InFutureEnum,
                Placecapacity = 100
            };
            _context.Add(organizer);
            _context.Add(session);
            _context.Add(eventToCancel);
            _context.SaveChanges();

            // Act
            var result = await _controller.CancelEvent(sessionToken, eventId);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(204, ((StatusCodeResult)result).StatusCode);
            Assert.Equal((int)EventStatus.CancelledEnum, _context.Events.First().Status);
        }

        // Add more tests for different scenarios in CancelEvent method

        [Fact]
        public async Task GetByCategory_ReturnsListOfEventsInCategory()
        {
            // Arrange
            int CategoriesId = 1;
            Category category = new Category
            {
                Id = CategoriesId,
                Name = "TestCategory"
            };
            Event event1 = new Event
            {
                Id = 1,
                Owner = 1,
                Title = "Test Event 1",
                Starttime = DateTime.UtcNow.AddHours(1),
                Endtime = DateTime.UtcNow.AddHours(2),
                Latitude = "12.345678",
                Longitude = "98.765432",
                Status = (int)EventStatus.InFutureEnum,
                Placecapacity = 100
            };
            Event event2 = new Event
            {
                Id = 2,
                Owner = 1,
                Title = "Test Event 2",
                Starttime = DateTime.UtcNow.AddHours(1),
                Endtime = DateTime.UtcNow.AddHours(2),
                Latitude = "12.345678",
                Longitude = "98.765432",
                Status = (int)EventStatus.InFutureEnum,
                Placecapacity = 100
            };
            Eventincategory eventInCategory1 = new Eventincategory
            {
                Id = 1,
                EventId = 1,
                CategoriesId = CategoriesId
            };
            Eventincategory eventInCategory2 = new Eventincategory
            {
                Id = 2,
                EventId = 2,
                CategoriesId = CategoriesId
            };
            _context.Add(category);
            _context.Add(event1);
            _context.Add(event2);
            _context.Add(eventInCategory1);
            _context.Add(eventInCategory2);
            _context.SaveChanges();

            // Act
            var result = await _controller.GetByCategory(CategoriesId);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, ((ObjectResult)result).StatusCode);
            List<EventDTO> events = ((ObjectResult)result).Value as List<EventDTO>;
            Assert.NotNull(events);
            Assert.Equal(2, events.Count);
            Assert.Equal("Test Event 1", events[0].Title);
            Assert.Equal("Test Event 2", events[1].Title);
        }

        // Add more tests for different scenarios in GetByCategory method

        // Add tests for GetEventById method
        [Fact]
        public async Task GetEventById_ReturnsEventWithGivenId()
        {
            // Arrange
            Event testEvent = new Event
            {
                Id = 1,
                Owner = 1,
                Title = "Test Event",
                Starttime = DateTime.UtcNow.AddHours(1),
                Endtime = DateTime.UtcNow.AddHours(2),
                Latitude = "12.345678",
                Longitude = "98.765432",
                Status = (int)EventStatus.InFutureEnum,
                Placecapacity = 100
            };
            _context.Add(testEvent);
            _context.SaveChanges();

            // Act
            var result = await _controller.GetEventById(testEvent.Id);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, ((ObjectResult)result).StatusCode);
            EventWithPlacesDTO returnedEvent = ((ObjectResult)result).Value as EventWithPlacesDTO;
            Assert.NotNull(returnedEvent);
            Assert.Equal(testEvent.Id, returnedEvent.Id);
            Assert.Equal(testEvent.Title, returnedEvent.Title);
        }

        // Add tests for GetEvents method

        // Add tests for GetMyEvents method
        [Fact]
        public async Task GetMyEvents_ReturnsListOfEventsCreatedByOrganizer()
        {
            // Arrange
            string sessionToken = "validSessionToken";
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = 1
            };
            Session session = new Session
            {
                Id = 1,
                Organizer = organizer,
                OrganizerId = 1,
                Time = DateTime.Now - TimeSpan.FromMinutes(10),
                Token = "validSessionToken"
            };
            Event event1 = new Event
            {
                Id = 1,
                Owner = 1,
                Title = "Test Event 1",
                Starttime = DateTime.UtcNow.AddHours(1),
                Endtime = DateTime.UtcNow.AddHours(2),
                Latitude = "12.345678",
                Longitude = "98.765432",
                Status = (int)EventStatus.InFutureEnum,
                Placecapacity = 100
            };
            Event event2 = new Event
            {
                Id = 2,
                Owner = 1,
                Title = "Test Event 2",
                Starttime = DateTime.UtcNow.AddHours(1),
                Endtime = DateTime.UtcNow.AddHours(2),
                Latitude = "12.345678",
                Longitude = "98.765432",
                Status = (int)EventStatus.InFutureEnum,
                Placecapacity = 100
            };
            _context.Add(organizer);
            _context.Add(session);
            _context.Add(event1);
            _context.Add(event2);
            _context.SaveChanges();

            // Act
            var result = await _controller.GetMyEvents(sessionToken);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, ((ObjectResult)result).StatusCode);
            List<EventDTO> events = ((ObjectResult)result).Value as List<EventDTO>;
            Assert.NotNull(events);
            Assert.Equal(2, events.Count);
            Assert.Equal("Test Event 1", events[0].Title);
            Assert.Equal("Test Event 2", events[1].Title);
        }

        [Fact]
        public async Task GetMyEvents_ReturnsStatusCode403_WhenInvalidSessionToken()
        {
            // Arrange
            string sessionToken = "invalidSessionToken";

            // Act
            var result = await _controller.GetMyEvents(sessionToken);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(403, ((StatusCodeResult)result).StatusCode);
        }

        // Add tests for PatchEvent method
        [Fact]
        public async Task PatchEvent_UpdatesTitle_ReturnsStatusCode202()
        {
            // Arrange
            string sessionToken = "validSessionToken";
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = 1
            };
            Session session = new Session
            {
                Id = 1,
                Organizer = organizer,
                OrganizerId = 1,
                Time = DateTime.Now - TimeSpan.FromMinutes(10),
                Token = "validSessionToken"
            };
            Event eventToUpdate = new Event
            {
                Id = 1,
                Owner = 1,
                Title = "Test Event",
                Starttime = DateTime.UtcNow.AddHours(1),
                Endtime = DateTime.UtcNow.AddHours(2),
                Latitude = "12.345678",
                Longitude = "98.765432",
                Status = (int)EventStatus.InFutureEnum,
                Placecapacity = 100
            };
            _context.Add(organizer);
            _context.Add(session);
            _context.Add(eventToUpdate);
            _context.SaveChanges();

            EventPatchDTO patchData = new EventPatchDTO
            {
                Title = "Updated Test Event"
            };

            // Act
            var result = await _controller.PatchEvent(sessionToken, eventToUpdate.Id.ToString(), patchData);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(202, ((StatusCodeResult)result).StatusCode);
            Assert.Equal("Updated Test Event", _context.Events.First().Title);
        }
 
        [Fact]
        public async Task PatchEvent_ReturnsStatusCode403_WhenInvalidSessionToken()
        {
            // Arrange
            string sessionToken = "invalidSessionToken";
            EventPatchDTO patchData = new EventPatchDTO
            {
                Title = "Updated Test Event"
            };

            // Act
            var result = await _controller.PatchEvent(sessionToken, "1", patchData);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(403, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task PatchEvent_ReturnsStatusCode404_WhenEventIdNotFound()
        {
            // Arrange
            string sessionToken = "validSessionToken";
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = 1
            };
            Session session = new Session
            {
                Id = 1,
                Organizer = organizer,
                OrganizerId = 1,
                Time = DateTime.Now - TimeSpan.FromMinutes(10),
                Token = "validSessionToken"
            };
            _context.Add(organizer);
            _context.Add(session);
            _context.SaveChanges();

            EventPatchDTO patchData = new EventPatchDTO
            {
                Title = "Updated Test Event"
            };

            // Act
            var result = await _controller.PatchEvent(sessionToken, "999", patchData);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, ((StatusCodeResult)result).StatusCode);
        }
    }
}