
using Data;
using Domain.Dtos;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using static Services.CommandHandlers.LoginCommandHandlers;

namespace Test
{
    public class LoginTest
    {
        [Fact]
        public async Task Handle_UserDoesNotExist_InsertsUser()
        {
            // Arrange
            var databaseMock = new Mock<IDatabaseConfig>();
            var configurationMock = new Mock<IConfiguration>();

            databaseMock.Setup(d => d.QueryAsync<LoginUser>(It.IsAny<string>(), null))
                .ReturnsAsync(new List<LoginUser>());
            databaseMock.Setup(d => d.InsertAsync(It.IsAny<string>(), null))
                .Returns(Task.FromResult(0));

            var handler = new RegisterUserCommandHandler(databaseMock.Object, configurationMock.Object);
            var request = new RegisterUserCommand
            {
                Username = "testuser",
                Password = "testpassword"
            };

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            databaseMock.Verify(d => d.QueryAsync<LoginUser>(It.IsAny<string>(), null), Times.Once);
            databaseMock.Verify(d => d.InsertAsync(It.IsAny<string>(), null), Times.Once);

            Assert.True(response.Result);
        }

        [Fact]
        public async Task Handle_UserAlreadyExists_ReturnsError()
        {
            // Arrange
            var databaseMock = new Mock<IDatabaseConfig>();
            var configurationMock = new Mock<IConfiguration>();

            databaseMock.Setup(d => d.QueryAsync<LoginUser>(It.IsAny<string>(), null))
                .ReturnsAsync(new List<LoginUser> {
                    new LoginUser { Username = "existinguser", Password = "123"
                    }});

            var handler = new RegisterUserCommandHandler(databaseMock.Object, configurationMock.Object);
            var request = new RegisterUserCommand
            {
                Username = "existinguser",
                Password = "testpassword"
            };

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            databaseMock.Verify(d => d.QueryAsync<LoginUser>(It.IsAny<string>(), null), Times.Once);
            databaseMock.Verify(d => d.InsertAsync(It.IsAny<string>(), null), Times.Never);

            Assert.False(response.Result);
        }
    }
}