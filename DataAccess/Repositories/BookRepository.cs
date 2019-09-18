﻿using Domain.Models;
using Domain.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class BookRepository : IRepository<Book>
    {
        private readonly string _connectionString;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(string connectionString, ILogger<BookRepository> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Book> Create(Book newModel)
        {
            Book book = null;
            try
            {
                using (var db = new SqliteConnection(_connectionString))
                {
                    db.Open();
                    using (var com = new SqliteCommand(
                        @"INSERT INTO books (
                            book_title, 
                            book_author, 
                            book_isbn, 
                            book_description,
                            book_checkedOut,
                            book_created,
                            book_updated,
                            book_createdBy
                        ) VALUES (
                            @title, 
                            @author, 
                            @isbn, 
                            @description,
                            @checkedOut,
                            @created,
                            @updated,
                            @createdBy
                        ); 
                        SELECT 
                            book_id, 
                            book_title, 
                            book_author, 
                            book_isbn, 
                            book_description ,
                            book_checkedOut,
                            book_created,
                            book_updated,
                            book_createdBy,
                            NULL
                        FROM books 
                        WHERE book_id = (SELECT MAX(book_id) FROM books);", db))
                    {
                        com.Parameters.Add(new SqliteParameter("title", newModel.Title));
                        com.Parameters.Add(new SqliteParameter("author", newModel.Author));
                        com.Parameters.Add(new SqliteParameter("isbn", newModel.ISBN));
                        com.Parameters.Add(new SqliteParameter("description", newModel.Description));
                        com.Parameters.Add(new SqliteParameter("checkedOut", newModel.CheckedOut));
                        com.Parameters.Add(new SqliteParameter("created", DateTime.UtcNow));
                        com.Parameters.Add(new SqliteParameter("updated", DateTime.UtcNow));
                        com.Parameters.Add(new SqliteParameter("createdBy", newModel.CreatedBy));

                        using (var reader = await com.ExecuteReaderAsync())
                        {
                            await reader.ReadAsync();
                            book = ReadBook(reader);
                        }
                    }
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not insert new book.");
            }
            return book;
        }

        public async Task<bool> Delete(int id)
        {
            bool deleted = false;
            try
            {
                using (var db = new SqliteConnection(_connectionString))
                {
                    db.Open();
                    using (var com = new SqliteCommand(
                        @"
                        DELETE
                        FROM userbooks
                        WHERE book_id = @id;
                        DELETE 
                        FROM books
                        WHERE book_id = @id", db))
                    {
                        com.Parameters.Add(new SqliteParameter("id", id));

                        deleted = await com.ExecuteNonQueryAsync() > 0;
                    }
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Could delete book {id}.");
            }

            return deleted;
        }

        public async Task<Book> Get(int id)
        {
            Book book = null;
            try
            {
                using (var db = new SqliteConnection(_connectionString))
                {
                    db.Open();
                    using (var com = new SqliteCommand(
                        @"SELECT 
                            b.book_id, 
                            b.book_title, 
                            b.book_author, 
                            b.book_isbn, 
                            b.book_description ,
                            b.book_checkedOut,
                            b.book_created,
                            b.book_updated,
                            b.book_createdBy,
                            u.user_username
                        FROM books AS b
                        LEFT JOIN userbooks AS ub
                            ON ub.book_id = b.book_id
                        LEFT JOIN users AS u
                            ON u.user_id = ub.user_id
                        WHERE b.book_id = @id", db))
                    {
                        com.Parameters.Add(new SqliteParameter("id", id));

                        using (var reader = await com.ExecuteReaderAsync())
                        {
                            await reader.ReadAsync();
                            if (reader.HasRows)
                            {
                                book = ReadBook(reader);
                            }
                        }
                    }
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Could not get book {id}.");
            }

            return book;
        }

        public async Task<IReadOnlyCollection<Book>> Get()
        {
            List<Book> books = new List<Book>();
            try
            {
                using (var db = new SqliteConnection(_connectionString))
                {
                    db.Open();
                    using (var com = new SqliteCommand(
                        @"SELECT 
                            b.book_id, 
                            b.book_title, 
                            b.book_author, 
                            b.book_isbn, 
                            b.book_description ,
                            b.book_checkedOut,
                            b.book_created,
                            b.book_updated,
                            b.book_createdBy,
                            u.user_username
                        FROM books AS b
                        LEFT JOIN userbooks AS ub
                            ON ub.book_id = b.book_id
                        LEFT JOIN users AS u
                            ON u.user_id = ub.user_id", db))
                    {
                        using (var reader = await com.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (reader.HasRows)
                                {
                                    books.Add(ReadBook(reader));
                                }
                            }
                        }
                    }
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not get all books.");
            }

            return books;
        }

        public async Task<Book> Update(int id, Book newModel)
        {
            Book book = null;
            try
            {
                using (var db = new SqliteConnection(_connectionString))
                {
                    db.Open();
                    using (var com = new SqliteCommand(
                        @"UPDATE books 
                            SET
                                book_title = @title, 
                                book_author = @author, 
                                book_isbn = @isbn, 
                                book_description = @description,
                                book_checkedOut = @checkedOut,
                                book_updated = @updated
                        WHERE book_id = @id;
                        SELECT 
                            b.book_id, 
                            b.book_title, 
                            b.book_author, 
                            b.book_isbn, 
                            b.book_description,
                            b.book_checkedOut,
                            b.book_created,
                            b.book_updated,
                            b.book_createdBy,
                            u.user_username
                        FROM books AS b
                        LEFT JOIN userbooks AS ub
                            ON ub.book_id = b.book_id
                        LEFT JOIN users AS u
                            ON u.user_id = ub.user_id
                        WHERE b.book_id = @id;", db))
                    {
                        com.Parameters.Add(new SqliteParameter("id", id));
                        com.Parameters.Add(new SqliteParameter("title", newModel.Title));
                        com.Parameters.Add(new SqliteParameter("author", newModel.Author));
                        com.Parameters.Add(new SqliteParameter("isbn", newModel.ISBN));
                        com.Parameters.Add(new SqliteParameter("description", newModel.Description));
                        com.Parameters.Add(new SqliteParameter("checkedOut", newModel.CheckedOut));
                        com.Parameters.Add(new SqliteParameter("updated", DateTime.UtcNow));

                        using (var reader = await com.ExecuteReaderAsync())
                        {
                            await reader.ReadAsync();
                            book = ReadBook(reader);
                        }
                    }
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Could not update book {id}.");
            }
            return book;
        }

        private static Book ReadBook(SqliteDataReader reader)
        {
            return new Book(
                id: reader.GetInt32(0),
                title: reader.GetString(1),
                author: reader.GetString(2),
                isbn: reader.GetString(3),
                description: reader.GetString(4),
                checkedOut: reader.GetBoolean(5),
                created: reader.GetDateTime(6),
                updated: reader.GetDateTime(7),
                createdBy: reader.GetString(8),
                checkedOutBy: !reader.IsDBNull(9) ? reader.GetString(9) : null
            );
        }

        public async Task<Book> Get(string id)
        {
            return await Get(Int32.Parse(id));
        }
    }
}
