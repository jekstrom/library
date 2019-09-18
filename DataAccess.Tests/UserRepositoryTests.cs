using DataAccess.Repositories;
using Domain.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xunit;

namespace DataAccess.Tests
{
    public class UserRepositoryTests
    {
        private readonly Mock<IDbConnection> _connection;
        private readonly Mock<ILogger<UserRepository>> _logger;
        private readonly IDictionary<string, IList<string>> _userRoleMap;

        public UserRepositoryTests()
        {
            _connection = new Mock<IDbConnection>();
            _logger = new Mock<ILogger<UserRepository>>();

            _userRoleMap = new Dictionary<string, IList<string>>
            {
                { "username", new List<string> { "superadmin" } }
            };
        }

        [Fact]
        public async void Create_ExecutesWithCorrectParameters()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            UserRepository repository = CreateRepository();

            DateTime now = DateTime.UtcNow; // TODO: inject date

            await repository.Create(
                new LibraryUser(1, "user", "username", new Book[] { })
            );

            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "name" && param.Value.ToString() == "user"))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "username" && param.Value.ToString() == "username"))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "roleName" && param.Value.ToString() == "superadmin"))
                , Times.Once
            );
        }

        private UserRepository CreateRepository()
        {
            return new UserRepository(_connection.Object, _userRoleMap, _logger.Object);
        }
    }
}
