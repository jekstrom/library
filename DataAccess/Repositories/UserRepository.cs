using Domain.Models;
using Domain.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UserRepository : IRepository<LibraryUser>
    {
        private readonly string _connectionString;
        private readonly IDictionary<string, IList<string>> _userRoleMap;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(string connectionString, IDictionary<string, IList<string>> userRoleMap, ILogger<UserRepository> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _userRoleMap = userRoleMap ?? throw new ArgumentNullException(nameof(userRoleMap));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LibraryUser> Create(LibraryUser newModel)
        {
            LibraryUser user = null;
            IList<string> roles = new List<string>();

            if (!_userRoleMap.TryGetValue(newModel.Username, out roles))
            {
                if (roles == null)
                {
                    roles = new List<string>();
                }
                roles.Add("guest");
            }

            try
            {
                using (var db = new SqliteConnection(_connectionString))
                {
                    db.Open();
                    foreach (string roleName in roles)
                    {
                        using (var com = new SqliteCommand(
                            @"INSERT INTO users (
                                user_name, 
                                user_username
                            ) 
                            SELECT
                                @name,
                                @username
                            WHERE NOT EXISTS (SELECT 1 FROM users WHERE user_name = @name AND user_username = @username);
                            INSERT INTO userroles (
                                user_id,
                                role_id
                            )
                            SELECT 
                                u.user_id,
                                r.role_id
                            FROM users AS u
                            CROSS JOIN roles AS r
                            WHERE 
                                u.user_id = (SELECT MAX(u2.user_id) FROM users AS u2)
                                AND r.role_value = @roleName
                                AND NOT EXISTS (SELECT 1 FROM userroles AS ur WHERE ur.user_id = u.user_id AND ur.role_id = r.role_id);
                            SELECT 
                                u.user_id,
                                u.user_name, 
                                u.user_username,
                                r.role_value,
                                0,
                                '',
                                '',
                                '',
                                ''
                            FROM users AS u
                            INNER JOIN userroles AS ur
                                ON ur.user_id = u.user_id
                            INNER JOIN roles AS r
                                ON r.role_id = ur.role_id
                            WHERE u.user_id = (SELECT MAX(u2.user_id) FROM users AS u2);", db))
                        {
                            com.Parameters.Add(new SqliteParameter("name", newModel.Name ?? "Anonymous"));
                            com.Parameters.Add(new SqliteParameter("username", newModel.Username));
                            com.Parameters.Add(new SqliteParameter("roleName", roleName));

                            using (var reader = await com.ExecuteReaderAsync())
                            {
                                user = await ReadUser(reader);
                            }
                        }
                    }
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not insert new user.");
            }
            return user;
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<LibraryUser>> Get()
        {
            throw new NotImplementedException();
        }

        public async Task<LibraryUser> Get(string id)
        {
            LibraryUser user = null;
            IList<string> roles = new List<string>();

            if (!_userRoleMap.TryGetValue(id, out roles))
            {
                if (roles == null)
                {
                    roles = new List<string>();
                }
                roles.Add("guest");
            }

            try
            {
                using (var db = new SqliteConnection(_connectionString))
                {
                    db.Open();
                    foreach (string roleName in roles)
                    {
                        using (var com = new SqliteCommand(
                            @"
                            SELECT 
                                u.user_id,
                                u.user_name, 
                                u.user_username,
                                r.role_value,
                                b.book_id,
                                b.book_title,
                                b.book_author,
                                b.book_isbn,
                                b.book_description
                            FROM users AS u
                            INNER JOIN userroles AS ur
                                ON ur.user_id = u.user_id
                            INNER JOIN roles AS r
                                ON r.role_id = ur.role_id
                            LEFT JOIN userbooks AS ub
                                ON ub.user_id = u.user_id   
                            LEFT JOIN books AS b
                                ON b.book_id = ub.book_id
                            WHERE u.user_username = @username;", db))
                        {
                            com.Parameters.Add(new SqliteParameter("username", id));

                            using (var reader = await com.ExecuteReaderAsync())
                            {
                                if (reader.HasRows)
                                {
                                    user = await ReadUser(reader);
                                }
                            }
                        }
                    }
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Could get user {id}.");
            }
            return user;
        }

        public Task<LibraryUser> Update(int id, LibraryUser newModel)
        {
            throw new NotImplementedException();
        }

        public Task<LibraryUser> Get(int id)
        {
            throw new NotImplementedException();
        }

        private static async Task<LibraryUser> ReadUser(SqliteDataReader reader)
        {
            LibraryUser user = null;
            while (await reader.ReadAsync())
            {
                int userId = reader.GetInt32(0);
                string name = reader.GetString(1);
                string username = reader.GetString(2);
                string role = reader.GetString(3);
                Book book = null;
                if (!reader.IsDBNull(4))
                {
                    book = new Book(
                        id: reader.GetInt32(4),
                        title: reader.GetString(5),
                        author: reader.GetString(6),
                        isbn: reader.GetString(7),
                        description: reader.GetString(8)
                    );
                }

                if (user is object)
                {
                    user = new LibraryUser(userId, name, username, user.Roles.Concat(new[] { role }).ToList(), user.BooksCheckedOut.Concat(new[] { book }).ToList());
                }
                else
                {
                    user = new LibraryUser(userId, name, username, new[] { role }, new[] { book });
                }
            }
            return user;
        }
    }
}
