using System;
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
    public class EventOrganizerApiControllerTests
    {
        private readonly DionizosDataContext _context;
        private readonly EventOrganizerApiController _controller;
        private readonly Mock<IHelper> _mockHelper;
        private readonly Mock<IMailing> _mockMailing;
        private readonly Mock<ILogger<EventOrganizerApiController>> _mockLogger;

        public EventOrganizerApiControllerTests()
        {
            _mockHelper = new Mock<IHelper>();
            _mockLogger = new Mock<ILogger<EventOrganizerApiController>>();
            _mockMailing = new Mock<IMailing>();

            var options = new DbContextOptionsBuilder<DionizosDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DionizosDataContext(options);

            _controller = new EventOrganizerApiController(_context, _mockHelper.Object, _mockMailing.Object, _mockLogger.Object);
        }


        [Fact]
        public async Task LoginOrganizer_ReturnsStatusCode200_WhenValidCredentials()
        {
            // Arrange
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = dionizos_backend_app.Extensions.Extensions.EncryptPass("password"),
                Status = (int)Organizer.StatusEnum.ConfirmedEnum
            };
            _context.Add(organizer);
            _context.SaveChanges();

            // Act
            var result = await _controller.LoginOrganizer(organizer.Email, "password");

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, ((ObjectResult)result).StatusCode);
        }
        [Fact]
        public async Task LoginOrganizer_ReturnsStatusCode400_WhenInvalidEmail()
        {
            // Arrange
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = (int)Organizer.StatusEnum.ConfirmedEnum
            };
            _context.Add(organizer);
            _context.SaveChanges();

            string invalidEmail = "wrong@example.com";

            // Act
            var result = await _controller.LoginOrganizer(invalidEmail, organizer.Password);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(400, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task LoginOrganizer_ReturnsStatusCode400_WhenInvalidPassword()
        {
            // Arrange
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = (int)Organizer.StatusEnum.ConfirmedEnum
            };
            _context.Add(organizer);
            _context.SaveChanges();

            string invalidPassword = "wrongPassword";

            // Act
            var result = await _controller.LoginOrganizer(organizer.Email, invalidPassword);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(400, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task LoginOrganizer_ReturnsStatusCode400_WhenUnconfirmedOrganizer()
        {
            // Arrange
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = (int)Organizer.StatusEnum.PendingEnum
            };
            _context.Add(organizer);
            _context.SaveChanges();

            // Act
            var result = await _controller.LoginOrganizer(organizer.Email, organizer.Password);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(400, ((StatusCodeResult)result).StatusCode);
        }

        // Add more tests for different scenarios in LoginOrganizer method

        [Fact]
        public async Task GetOrganizer_ReturnsStatusCode200_WhenValidSession()
        {
            // Arrange
            string sessionToken = "validSessionToken";
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = (int)Organizer.StatusEnum.ConfirmedEnum
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

            _mockHelper.Setup(x => x.Validate(sessionToken)).Returns(organizer);

            // Act
            var result = _controller.GetOrganizer(sessionToken);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, ((ObjectResult)result).StatusCode);
        }
        [Fact]
        public void GetOrganizer_ReturnsStatusCode403_WhenInvalidSession()
        {
            // Arrange
            string sessionToken = "invalidSessionToken";

            _mockHelper.Setup(x => x.Validate(sessionToken)).Returns((Organizer)null);

            // Act
            var result = _controller.GetOrganizer(sessionToken);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(403, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public void GetOrganizer_ReturnsStatusCode403_WhenExpiredSession()
        {
            // Arrange
            string sessionToken = "expiredSessionToken";
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = (int)Organizer.StatusEnum.ConfirmedEnum
            };
            Session session = new Session
            {
                Id = 1,
                Organizer = organizer,
                OrganizerId = 1,
                Time = DateTime.Now - TimeSpan.FromDays(2), // Expired session
                Token = "expiredSessionToken"
            };
            _context.Add(organizer);
            _context.Add(session);
            _context.SaveChanges();

            _mockHelper.Setup(x => x.Validate(sessionToken)).Returns((Organizer)null);

            // Act
            var result = _controller.GetOrganizer(sessionToken);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(403, ((StatusCodeResult)result).StatusCode);
        }


        // Add tests for Confirm method
        [Fact]
        public async Task Confirm_ReturnsStatusCode202_WhenValidCode()
        {
            // Arrange
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = (int)Organizer.StatusEnum.PendingEnum
            };
            Emailcode emailCode = new Emailcode
            {
                Id = 1,
                OrganizerId = organizer.Id,
                Code = "123456",
                Time = DateTime.Now - TimeSpan.FromMinutes(10)
            };
            _context.Add(organizer);
            _context.Add(emailCode);
            _context.SaveChanges();

            // Act
            var result = await _controller.Confirm(organizer.Id.ToString(), emailCode.Code);

            // Assert
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(202, ((ObjectResult)result).StatusCode);
        }

        [Fact]
        public async Task Confirm_ReturnsStatusCode400_WhenInvalidCode()
        {
            // Arrange
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = (int)Organizer.StatusEnum.PendingEnum
            };
            Emailcode emailCode = new Emailcode
            {
                Id = 1,
                OrganizerId = organizer.Id,
                Code = "123456",
                Time = DateTime.Now - TimeSpan.FromMinutes(10)
            };
            _context.Add(organizer);
            _context.Add(emailCode);
            _context.SaveChanges();

            // Act
            var result = await _controller.Confirm(organizer.Id.ToString(), "000000");

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(400, ((StatusCodeResult)result).StatusCode);
        }
        // Add tests for DeleteOrganizer method
        [Fact]
        public async Task DeleteOrganizer_ReturnsStatusCode204_WhenValidSessionAndNoPlannedOrPendingEvents()
        {
            // Arrange
            string sessionToken = "validSessionToken";
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = (int)Organizer.StatusEnum.ConfirmedEnum
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

            _mockHelper.Setup(x => x.Validate(sessionToken)).Returns(organizer);

            // Act
            var result = await _controller.DeleteOrganizer(sessionToken, organizer.Id.ToString());

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(204, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task DeleteOrganizer_ReturnsStatusCode403_WhenInvalidSession()
        {
            // Arrange
            string sessionToken = "invalidSessionToken";

            // Act
            var result = await _controller.DeleteOrganizer(sessionToken, "1");

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(403, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task DeleteOrganizer_ReturnsStatusCode404_WhenTryingToDeleteOtherOrganizer()
        {
            // Arrange
            string sessionToken = "validSessionToken";
            Organizer organizer1 = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = (int)Organizer.StatusEnum.ConfirmedEnum
            };
            Organizer organizer2 = new Organizer
            {
                Id = 2,
                Name = "Jane Doe",
                Email = "jane@example.com",
                Password = "password",
                Status = (int)Organizer.StatusEnum.ConfirmedEnum
            };
            Session session = new Session
            {
                Id = 1,
                Organizer = organizer1,
                OrganizerId = 1,
                Time = DateTime.Now - TimeSpan.FromMinutes(10),
                Token = "validSessionToken"
            };
            _context.Add(organizer1);
            _context.Add(organizer2);
            _context.Add(session);
            _context.SaveChanges();

            _mockHelper.Setup(x => x.Validate(sessionToken)).Returns(organizer1);

            // Act
            var result = await _controller.DeleteOrganizer(sessionToken, organizer2.Id.ToString());

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task DeleteOrganizer_ReturnsStatusCode404_WhenPlannedOrPendingEventsExist()
        {
            // Arrange
            string sessionToken = "validSessionToken";
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = (int)Organizer.StatusEnum.ConfirmedEnum
            };
            Event event1 = new Event
            {
                Id = 1,
                Owner = organizer.Id,
                Status = (int)EventStatus.InFutureEnum,
                Starttime = DateTime.Now.AddHours(2),
                Endtime = DateTime.Now.AddHours(4),
                Title = "Test Event",
                Name = "Test Event",
                Latitude = "0",
                Longitude = "0",
                Placecapacity = 100
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
            _context.Add(event1);
            _context.Add(session);
            _context.SaveChanges();

            _mockHelper.Setup(x => x.Validate(sessionToken)).Returns(organizer);

            // Act
            var result = await _controller.DeleteOrganizer(sessionToken, organizer.Id.ToString());

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, ((StatusCodeResult)result).StatusCode);
        }
        // Add tests for PatchOrganizer method
        [Fact]
        public async Task PatchOrganizer_ReturnsStatusCode403_WhenInvalidSession()
        {
            // Arrange
            string sessionToken = "invalidSessionToken";
            string organizerId = "1";
            OrganizerPatchDTO organizerPatch = new OrganizerPatchDTO
            {
                Name = "John Doe",
                Password = "new_password"
            };

            _mockHelper.Setup(x => x.Validate(sessionToken)).Returns((Organizer)null);

            // Act
            var result = await _controller.PatchOrganizer(sessionToken, organizerId, organizerPatch);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(403, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task PatchOrganizer_ReturnsStatusCode404_WhenOrganizerNotFound()
        {
            // Arrange
            string sessionToken = "validSessionToken";
            string organizerId = "2";
            OrganizerPatchDTO organizerPatch = new OrganizerPatchDTO
            {
                Name = "John Doe",
                Password = "new_password"
            };
            
            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = (int)Organizer.StatusEnum.ConfirmedEnum
            };

            _context.Add(organizer);
            _context.SaveChanges();

            _mockHelper.Setup(x => x.Validate(sessionToken)).Returns(organizer);

            // Act
            var result = await _controller.PatchOrganizer(sessionToken, organizerId, organizerPatch);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task PatchOrganizer_ReturnsStatusCode202_WhenOrganizerUpdated()
        {
            // Arrange
            string sessionToken = "validSessionToken";
            string organizerId = "1";
            OrganizerPatchDTO organizerPatch = new OrganizerPatchDTO
            {
                Name = "John Doe",
                Password = "new_password"
            };

            Organizer organizer = new Organizer
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                Password = "password",
                Status = (int)Organizer.StatusEnum.ConfirmedEnum
            };

            _context.Add(organizer);
            _context.SaveChanges();

            _mockHelper.Setup(x => x.Validate(sessionToken)).Returns(organizer);

            // Act
            var result = await _controller.PatchOrganizer(sessionToken, organizerId, organizerPatch);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(202, ((StatusCodeResult)result).StatusCode);
        }
    }
}