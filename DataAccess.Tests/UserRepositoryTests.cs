using DataAccess.Repositories;
using Domain.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
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

        [Fact]
        public async void Create_ReturnsUser()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();
            Mock<IDataReader> reader = new Mock<IDataReader>();

            bool called = false;
            reader.Setup(r => r.Read()).Returns(() => { bool result = !called; called = true; return result; });
            reader.Setup(r => r.GetInt32(0)).Returns(1);
            reader.Setup(r => r.GetString(1)).Returns("name");
            reader.Setup(r => r.GetString(2)).Returns("username");
            reader.Setup(r => r.GetString(3)).Returns("superadmin");
            reader.Setup(r => r.IsDBNull(4)).Returns(true);

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            command.Setup(c => c.ExecuteReader()).Returns(reader.Object);
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            var expected = new LibraryUser(1, "name", "username", new[] { "superadmin" }, new Book[] { });

            UserRepository repository = CreateRepository();

            DateTime now = DateTime.UtcNow;

            LibraryUser actual = await repository.Create(
                new LibraryUser(1, "user", "username", new Book[] { })
            );

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void Create_ThrowsException_Logged()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            command.Setup(c => c.ExecuteReader()).Throws(new Exception("Bang!"));
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            var expected = new LibraryUser(1, "name", "username", new[] { "superadmin" }, new Book[] { });

            UserRepository repository = CreateRepository();

            DateTime now = DateTime.UtcNow;

            await repository.Create(
                new LibraryUser(1, "user", "username", new Book[] { })
            );

            _logger.Verify(l =>
                l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<FormattedLogValues>(),
                    It.Is<Exception>(e => e.Message.Contains("Bang!")),
                    It.IsAny<Func<object, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async void GetWithUsername_ExecutesWithCorrectParameters()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            UserRepository repository = CreateRepository();

            await repository.Get("username");

            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "username" && param.Value.ToString() == "username"))
                , Times.Once
            );
        }

        [Fact]
        public async void GetWithUsername_ReturnsUser()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();
            Mock<IDataReader> reader = new Mock<IDataReader>();

            bool called = false;
            reader.Setup(r => r.Read()).Returns(() => { bool result = !called; called = true; return result; });
            reader.Setup(r => r.GetInt32(0)).Returns(1);
            reader.Setup(r => r.GetString(1)).Returns("name");
            reader.Setup(r => r.GetString(2)).Returns("username");
            reader.Setup(r => r.GetString(3)).Returns("superadmin");
            reader.Setup(r => r.IsDBNull(4)).Returns(true);

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            command.Setup(c => c.ExecuteReader()).Returns(reader.Object);
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            var expected = new LibraryUser(1, "name", "username", new[] { "superadmin" }, new Book[] { });

            UserRepository repository = CreateRepository();

            LibraryUser actual = await repository.Get("username");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void GetWithUsername_ThrowsException_Logged()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            command.Setup(c => c.ExecuteReader()).Throws(new Exception("Bang!"));
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            var expected = new LibraryUser(1, "name", "username", new[] { "superadmin" }, new Book[] { });

            UserRepository repository = CreateRepository();

            await repository.Get("username");

            _logger.Verify(l =>
                l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<FormattedLogValues>(),
                    It.Is<Exception>(e => e.Message.Contains("Bang!")),
                    It.IsAny<Func<object, Exception, string>>()
                ),
                Times.Once
            );
        }

        private UserRepository CreateRepository()
        {
            return new UserRepository(_connection.Object, _userRoleMap, _logger.Object);
        }
    }
}
