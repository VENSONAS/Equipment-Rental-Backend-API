using Xunit;
using Moq;
using FluentAssertions;
using Service;
using Domain.Models;
using Repository;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Test.ServiceTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _repo;
        private readonly Mock<ILogger<UserService>> _logger;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _repo = new Mock<IUserRepository>();
            _logger = new Mock<ILogger<UserService>>();

            _service = new UserService(_repo.Object, _logger.Object);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnAllUsers()
        {
            var users = new List<User>
        {
            CreateUser(1, "A"),
            CreateUser(2, "B")
        };

            _repo.Setup(r => r.GetAllUsers()).ReturnsAsync(users);

            var result = await _service.GetAllUsers();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser()
        {
            var user = CreateUser(1, "Test");

            _repo.Setup(r => r.GetUserById(1)).ReturnsAsync(user);

            var result = await _service.GetUserById(1);

            result.Should().Be(user);
        }

        [Fact]
        public async Task CreateUser_ShouldSetCreatedTime_AndReturnCreatedUser()
        {
            var input = CreateUser(0, "Mike");
            var created = CreateUser(5, "Mike");

            _repo.Setup(r => r.CreateUser(input)).ReturnsAsync(created);

            var result = await _service.CreateUser(input);

            result.Id.Should().Be(5);
            result.CreatedAt.Should().NotBe(default); // must be set by domain method

            _logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateUser_ShouldModifyNameAndEmail_AndSave()
        {
            var original = CreateUser(1, "Old");
            original.ChangeEmail("old@mail.com");

            var updated = CreateUser(0, "New");
            updated.ChangeEmail("new@mail.com");

            _repo.Setup(r => r.GetUserById(1)).ReturnsAsync(original);
            _repo.Setup(r => r.UpdateUser(original)).ReturnsAsync(original);

            var result = await _service.UpdateUser(1, updated);

            result.Name.Should().Be("New");
            result.Email.Should().Be("new@mail.com");

            _repo.Verify(r => r.UpdateUser(original), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldCallRepository()
        {
            _repo.Setup(r => r.DeleteUser(7)).Returns(Task.CompletedTask);

            await _service.DeleteUser(7);

            _repo.Verify(r => r.DeleteUser(7), Times.Once);
        }

        private User CreateUser(int id, string name)
        {
            var u = new User();
            u.ChangeName(name);
            u.ChangeEmail($"{name}@mail.com");

            typeof(User).GetProperty("Id")!.SetValue(u, id);
            typeof(User).GetProperty("Role")!.SetValue(u, UserRole.Customer);

            return u;
        }
    }
}
