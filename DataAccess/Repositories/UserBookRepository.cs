using Domain.Models;
using Domain.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UserBookRepository : IUserBookRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UserBookRepository> _logger;

        public UserBookRepository(string connectionString, ILogger<UserBookRepository> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddUserBook(Book book)
        {
            try
            {
                using (var db = new SqliteConnection(_connectionString))
                {
                    db.Open();
                    using (var com = new SqliteCommand(
                        @"INSERT INTO userbooks (
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
                          WHERE book_id = @bookId;", db))
                    {
                        com.Parameters.Add(new SqliteParameter("username", book.CheckedOutBy));
                        com.Parameters.Add(new SqliteParameter("bookId", book.Id));

                        await com.ExecuteNonQueryAsync();
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
                using (var db = new SqliteConnection(_connectionString))
                {
                    db.Open();
                    using (var com = new SqliteCommand(
                        @"DELETE
                          FROM userbooks 
                          WHERE user_id IN (
                            SELECT user_id
                            FROM users
                            WHERE user_username = @username
                          )
                          AND book_id = @bookId;
                          UPDATE books
                            SET book_checkedOut = 0
                          WHERE book_id = @bookId;", db))
                    {
                        com.Parameters.Add(new SqliteParameter("username", book.CheckedOutBy));
                        com.Parameters.Add(new SqliteParameter("bookId", book.Id));

                        await com.ExecuteNonQueryAsync();
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
