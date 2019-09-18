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
    public class BookRepositoryTests
    {
        private readonly Mock<IDbConnection> _connection;
        private readonly Mock<ILogger<BookRepository>> _logger;

        public BookRepositoryTests()
        {
            _connection = new Mock<IDbConnection>();
            _logger = new Mock<ILogger<BookRepository>>();
        }

        [Fact]
        public async void Create_ExecutesWithCorrectParameters()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            BookRepository repository = CreateRepository();

            DateTime now = DateTime.UtcNow; // TODO: inject date

            await repository.Create(
                new Book(
                    1, 
                    "book title", 
                    "book author", 
                    "book isbn", 
                    "book description", 
                    true, 
                    now, 
                    now, 
                    "book created by", 
                    "book checked out by"
                    )
                );

            parameters.Verify(p => 
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "title" && param.Value.ToString() == "book title"))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "author" && param.Value.ToString() == "book author"))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "isbn" && param.Value.ToString() == "book isbn"))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "description" && param.Value.ToString() == "book description"))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "checkedOut" && (bool)param.Value))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "created" && ((DateTime)param.Value).ToString("yyyy-MM-dd") == now.ToString("yyyy-MM-dd")))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "updated" && ((DateTime)param.Value).ToString("yyyy-MM-dd") == now.ToString("yyyy-MM-dd")))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "createdBy" && param.Value.ToString() == "book created by"))
                , Times.Once
            );
        }

        [Fact]
        public async void Create_ReturnsBook()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();
            Mock<IDataReader> reader = new Mock<IDataReader>();

            DateTime now = DateTime.UtcNow;

            reader.Setup(r => r.GetInt32(0)).Returns(1);
            reader.Setup(r => r.GetString(1)).Returns("book title");
            reader.Setup(r => r.GetString(2)).Returns("book author");
            reader.Setup(r => r.GetString(3)).Returns("book isbn");
            reader.Setup(r => r.GetString(4)).Returns("book description");
            reader.Setup(r => r.GetBoolean(5)).Returns(true);
            reader.Setup(r => r.GetDateTime(6)).Returns(now);
            reader.Setup(r => r.GetDateTime(7)).Returns(now);
            reader.Setup(r => r.GetString(8)).Returns("book created by");
            reader.Setup(r => r.GetString(9)).Returns("book checked out by");

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            command.Setup(c => c.ExecuteReader()).Returns(reader.Object);
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            Book expected = new Book(
                1,
                "book title",
                "book author",
                "book isbn",
                "book description",
                true,
                now,
                now,
                "book created by",
                "book checked out by"
            );

            BookRepository repository = CreateRepository();

            Book actual = await repository.Create(
               new Book(
                       1,
                       "book title",
                       "book author",
                       "book isbn",
                       "book description",
                       true,
                       now,
                       now,
                       "book created by",
                       "book checked out by"
                   )
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

            BookRepository repository = CreateRepository();

            DateTime now = DateTime.UtcNow;

            await repository.Create(
                new Book(
                    1,
                    "book title",
                    "book author",
                    "book isbn",
                    "book description",
                    true,
                    now,
                    now,
                    "book created by",
                    "book checked out by"
                    )
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
        public async void Delete_ExecutesWithCorrectParameters()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            BookRepository repository = CreateRepository();

            DateTime now = DateTime.UtcNow;

            await repository.Delete(42);

            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "id" && (int)param.Value == 42))
                , Times.Once
            );
        }

        [Fact]
        public async void Delete_ReturnsTrue_OnSuccess()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            command.Setup(c => c.ExecuteNonQuery()).Returns(1);
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            BookRepository repository = CreateRepository();

            DateTime now = DateTime.UtcNow;

            bool actual = await repository.Delete(42);

            Assert.True(actual);
        }

        [Fact]
        public async void Delete_ThrowsException_Logged()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            command.Setup(c => c.ExecuteNonQuery()).Throws(new Exception("Bang!"));
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            BookRepository repository = CreateRepository();

            DateTime now = DateTime.UtcNow;

            await repository.Delete(42);

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
        public async void GetWithId_ExecutesWithCorrectParameters()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            BookRepository repository = CreateRepository();

            DateTime now = DateTime.UtcNow;

            await repository.Get(42);

            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "id" && (int)param.Value == 42))
                , Times.Once
            );
        }

        [Fact]
        public async void GetWithId_ReturnsBook()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();
            Mock<IDataReader> reader = new Mock<IDataReader>();

            DateTime now = DateTime.UtcNow;

            reader.Setup(r => r.Read()).Returns(true);
            reader.Setup(r => r.GetInt32(0)).Returns(42);
            reader.Setup(r => r.GetString(1)).Returns("book title");
            reader.Setup(r => r.GetString(2)).Returns("book author");
            reader.Setup(r => r.GetString(3)).Returns("book isbn");
            reader.Setup(r => r.GetString(4)).Returns("book description");
            reader.Setup(r => r.GetBoolean(5)).Returns(true);
            reader.Setup(r => r.GetDateTime(6)).Returns(now);
            reader.Setup(r => r.GetDateTime(7)).Returns(now);
            reader.Setup(r => r.GetString(8)).Returns("book created by");
            reader.Setup(r => r.GetString(9)).Returns("book checked out by");

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            command.Setup(c => c.ExecuteReader()).Returns(reader.Object);
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            Book expected = new Book(
                42,
                "book title",
                "book author",
                "book isbn",
                "book description",
                true,
                now,
                now,
                "book created by",
                "book checked out by"
            );

            BookRepository repository = CreateRepository();

            Book actual = await repository.Get(42);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void GetWithId_ThrowsException_Logged()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            command.Setup(c => c.ExecuteReader()).Throws(new Exception("Bang!"));
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            BookRepository repository = CreateRepository();

            DateTime now = DateTime.UtcNow;

            await repository.Get(42);

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
        public async void Get_ReturnsBooks()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();
            Mock<IDataReader> reader = new Mock<IDataReader>();

            DateTime now = DateTime.UtcNow;
            bool called = false;
            reader.Setup(r => r.Read()).Returns(() => { bool result = !called; called = true; return result; });
            reader.Setup(r => r.GetInt32(0)).Returns(42);
            reader.Setup(r => r.GetString(1)).Returns("book title");
            reader.Setup(r => r.GetString(2)).Returns("book author");
            reader.Setup(r => r.GetString(3)).Returns("book isbn");
            reader.Setup(r => r.GetString(4)).Returns("book description");
            reader.Setup(r => r.GetBoolean(5)).Returns(true);
            reader.Setup(r => r.GetDateTime(6)).Returns(now);
            reader.Setup(r => r.GetDateTime(7)).Returns(now);
            reader.Setup(r => r.GetString(8)).Returns("book created by");
            reader.Setup(r => r.GetString(9)).Returns("book checked out by");

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            command.Setup(c => c.ExecuteReader()).Returns(reader.Object);
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            IReadOnlyCollection<Book> expected = new[] {
                new Book(
                    42,
                    "book title",
                    "book author",
                    "book isbn",
                    "book description",
                    true,
                    now,
                    now,
                    "book created by",
                    "book checked out by"
                )
            };

            BookRepository repository = CreateRepository();

            IReadOnlyCollection<Book> actual = await repository.Get();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void Get_ThrowsException_Logged()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            command.Setup(c => c.ExecuteReader()).Throws(new Exception("Bang!"));
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            BookRepository repository = CreateRepository();

            DateTime now = DateTime.UtcNow;

            await repository.Get();

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
        public async void Update_ExecutesWithCorrectParameters()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            BookRepository repository = CreateRepository();

            DateTime now = DateTime.UtcNow; // TODO: inject date

            await repository.Update(
                1,
                new Book(
                    1,
                    "book title",
                    "book author",
                    "book isbn",
                    "book description",
                    true,
                    now,
                    now,
                    "book created by",
                    "book checked out by"
                    )
                );

            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "id" && (int)param.Value == 1))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "title" && param.Value.ToString() == "book title"))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "author" && param.Value.ToString() == "book author"))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "isbn" && param.Value.ToString() == "book isbn"))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "description" && param.Value.ToString() == "book description"))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "checkedOut" && (bool)param.Value))
                , Times.Once
            );
            parameters.Verify(p =>
                p.Add(
                    It.Is<SqliteParameter>(param => param.ParameterName == "updated" && ((DateTime)param.Value).ToString("yyyy-MM-dd") == now.ToString("yyyy-MM-dd")))
                , Times.Once
            );
        }

        [Fact]
        public async void Update_ReturnsBook()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();
            Mock<IDataReader> reader = new Mock<IDataReader>();

            DateTime now = DateTime.UtcNow;

            reader.Setup(r => r.Read()).Returns(true);
            reader.Setup(r => r.GetInt32(0)).Returns(1);
            reader.Setup(r => r.GetString(1)).Returns("book title");
            reader.Setup(r => r.GetString(2)).Returns("book author");
            reader.Setup(r => r.GetString(3)).Returns("book isbn");
            reader.Setup(r => r.GetString(4)).Returns("book description");
            reader.Setup(r => r.GetBoolean(5)).Returns(true);
            reader.Setup(r => r.GetDateTime(6)).Returns(now);
            reader.Setup(r => r.GetDateTime(7)).Returns(now);
            reader.Setup(r => r.GetString(8)).Returns("book created by");
            reader.Setup(r => r.GetString(9)).Returns("book checked out by");

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            command.Setup(c => c.ExecuteReader()).Returns(reader.Object);
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            Book expected = new Book(
                1,
                "book title",
                "book author",
                "book isbn",
                "book description",
                true,
                now,
                now,
                "book created by",
                "book checked out by"
            );

            BookRepository repository = CreateRepository();

            Book actual = await repository.Update(
               1,
               new Book(
                       1,
                       "book title",
                       "book author",
                       "book isbn",
                       "book description",
                       true,
                       now,
                       now,
                       "book created by",
                       "book checked out by"
                   )
               );

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void Update_ThrowsException_Logged()
        {
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> parameters = new Mock<IDataParameterCollection>();

            command.SetupGet(c => c.Parameters).Returns(parameters.Object);
            command.Setup(c => c.ExecuteReader()).Throws(new Exception("Bang!"));
            _connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            BookRepository repository = CreateRepository();

            DateTime now = DateTime.UtcNow;

            await repository.Update(
                1,
                new Book(
                    1,
                    "book title",
                    "book author",
                    "book isbn",
                    "book description",
                    true,
                    now,
                    now,
                    "book created by",
                    "book checked out by"
                    )
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

        private BookRepository CreateRepository()
        {
            return new BookRepository(_connection.Object, _logger.Object);
        }
    }
}
