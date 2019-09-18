using Domain.Models;
using Domain.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UserBookRepository : IUserBookRepository
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<UserBookRepository> _logger;

        public UserBookRepository(IDbConnection connection, ILogger<UserBookRepository> logger)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddUserBook(Book book)
        {
            try
            {
                using (var db = _connection)
                {
                    db.Open();
                    using (var com = _connection.CreateCommand())
                    {
                        com.CommandText = @"INSERT INTO userbooks (
                            user_id,
                            book_id
                          )
                          SELECT 
                            user_id,
                            @bookId
                          FROM users
                          WHERE user_username = @username;
                          UPDATE books
                            SET book_checkedOut = 1
                          WHERE book_id = @bookId;";

                        com.Parameters.Add(new SqliteParameter("username", book.CheckedOutBy));
                        com.Parameters.Add(new SqliteParameter("bookId", book.Id));

                        com.ExecuteNonQuery();
                    }
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Could insert user book for user {book.CheckedOutBy} and book {book.Id}.");
            }
        }

        public async Task RemoveUserBook(Book book)
        {
            try
            {
                using (var db = _connection)
                {
                    db.Open();
                    using (var com = _connection.CreateCommand())
                    {
                        com.CommandText = @"DELETE
                          FROM userbooks 
                          WHERE user_id IN (
                            SELECT user_id
                            FROM users
                            WHERE user_username = @username
                          )
                          AND book_id = @bookId;
                          UPDATE books
                            SET book_checkedOut = 0
                          WHERE book_id = @bookId;";

                        com.Parameters.Add(new SqliteParameter("username", book.CheckedOutBy));
                        com.Parameters.Add(new SqliteParameter("bookId", book.Id));

                        com.ExecuteNonQuery();
                    }
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Could remove user book for user {book.CheckedOutBy} and book {book.Id}.");
            }
        }
    }
}
