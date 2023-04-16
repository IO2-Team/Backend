using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using dionizos_backend_app;
using dionizos_backend_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.OpenAPITools.Controllers;
using Microsoft.EntityFrameworkCore;

using Moq;
using Org.OpenAPITools.Models;

namespace Tests
{
    public class CategoriesApiControllerTests
    {
    private readonly DionizosDataContext _context;
    private readonly CategoriesApiController _controller;
    private readonly Mock<IConfigurationRoot> _mockConfig;
    private readonly Mock<IHelper> _mockHelper;
        public CategoriesApiControllerTests()
    {
        _mockConfig = new Mock<IConfigurationRoot>();
        _mockHelper = new Mock<IHelper>();
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

        _controller = new CategoriesApiController(_context, _mockConfig.Object, _mockHelper.Object);
    }

    [Fact]
    public async Task AddCategories_ReturnsStatusCode201_WhenCategoryIsCreated()
    {
        // Arrange
        string sessionToken = "validSessionToken";
        string categoryName = "TestCategory";
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
        
        // Act
        var result = await _controller.AddCategories(sessionToken, categoryName);

        // Assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, ((ObjectResult)result).StatusCode);
        Assert.Single(_context.Categories);
    }

    [Fact]
    public async Task AddCategories_ReturnsStatusCode400_WhenCategoryAlreadyExists()
    {
        // Arrange
        string sessionToken = "validSessionToken";
        string categoryName = "TestCategory";
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
        _context.Categories.Add(new Category { Name = categoryName });
        _context.SaveChanges();

        // Act
        var result = await _controller.AddCategories(sessionToken, categoryName);

        // Assert
        Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(400, ((StatusCodeResult)result).StatusCode);
    }

    [Fact]
    public async Task AddCategories_ReturnsStatusCode403_WhenSessionTokenIsInvalid()
    {
        // Arrange
        string sessionToken = "invalidSessionToken";
        string categoryName = "TestCategory";

        // Act
        var result = await _controller.AddCategories(sessionToken, categoryName);

        // Assert
        Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(403, ((StatusCodeResult)result).StatusCode);
    }

    [Fact]
    public async Task AddCategories_ReturnsStatusCode400_WhenCategoryNameLengthIsLessThanTwo()
    {
        // Arrange
        string sessionToken = "validSessionToken";
        string categoryName = "1";
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

        // Act
        var result = await _controller.AddCategories(sessionToken, categoryName);

        // Assert
        Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(400, ((StatusCodeResult)result).StatusCode);
    }

    [Fact]
    public async Task GetCategories_ReturnsListOfCategories()
    {
        // Arrange
        Category cat1 = new Category { Name = "TestCategory1" };
        Category cat2 = new Category { Name = "TestCategory2" };
        _context.Categories.AddRange(cat1, cat2);
        _context.SaveChanges();

        // Act
        var result = await _controller.GetCategories();

        // Assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, ((ObjectResult)result).StatusCode);
        List<CategoryDTO> categories = ((ObjectResult)result).Value as List<CategoryDTO>;
        Assert.NotNull(categories);
        Assert.Equal(2, categories.Count);
        Assert.Equal("TestCategory1", categories[0].Name);
        Assert.Equal("TestCategory2", categories[1].Name);
    }

    }
}
