using Microsoft.Data.Sqlite;

namespace library.DatabaseInitialization
{
    public static class Tables
    {
        internal static void CreateBooksTable(string connectionString)
        {
            using (var db = new SqliteConnection(connectionString))
            {
                db.Open();
                using (var com = new SqliteCommand(
                    @"CREATE TABLE IF NOT EXISTS books (
                        book_id INTEGER PRIMARY KEY, 
                        book_title TEXT, 
                        book_author TEXT, 
                        book_isbn TEXT, 
                        book_description TEXT,
                        book_checkedOut INTEGER,
                        book_created TEXT,
                        book_updated TEXT,
                        book_createdBy TEXT
                    );", db))
                {
                    com.ExecuteNonQuery();
                }
                db.Close();
            }
        }

        internal static void CreateUsersTable(string connectionString)
        {
            using (var db = new SqliteConnection(connectionString))
            {
                db.Open();
                using (var com = new SqliteCommand(
                    @"CREATE TABLE IF NOT EXISTS roles (
                        role_id INTEGER PRIMARY KEY, 
                        role_value TEXT
                    );
                    INSERT INTO roles (role_value)
                    SELECT 'superadmin'
                    WHERE NOT EXISTS (SELECT 1 FROM roles WHERE role_value = 'superadmin');

                    INSERT INTO roles (role_value)
                    SELECT 'bookreader'
                    WHERE NOT EXISTS (SELECT 1 FROM roles WHERE role_value = 'bookreader');

                    INSERT INTO roles (role_value)
                    SELECT 'bookwriter'
                    WHERE NOT EXISTS (SELECT 1 FROM roles WHERE role_value = 'bookwriter');

                    INSERT INTO roles (role_value)
                    SELECT 'bookchecker'
                    WHERE NOT EXISTS (SELECT 1 FROM roles WHERE role_value = 'bookchecker');

                    INSERT INTO roles (role_value)
                    SELECT 'guest'
                    WHERE NOT EXISTS (SELECT 1 FROM roles WHERE role_value = 'guest');", db))
                {
                    com.ExecuteNonQuery();

                    com.CommandText =
                        @"CREATE TABLE IF NOT EXISTS users (
                            user_id INTEGER PRIMARY KEY, 
                            user_name TEXT, 
                            user_username TEXT
                        );";
                    com.ExecuteNonQuery();

                    com.CommandText =
                        @"CREATE TABLE IF NOT EXISTS userroles (
                            user_id INTEGER,
                            role_id INTEGER,
                            FOREIGN KEY(user_id) REFERENCES users(user_id),
                            FOREIGN KEY(role_id) REFERENCES roles(role_id)
                        )";
                    com.ExecuteNonQuery();

                    com.ExecuteNonQuery();

                    com.CommandText =
                        @"CREATE TABLE IF NOT EXISTS userbooks (
                            user_id INTEGER,
                            book_id INTEGER,
                            FOREIGN KEY(user_id) REFERENCES users(user_id),
                            FOREIGN KEY(book_id) REFERENCES books(book_id)
                        )";
                    com.ExecuteNonQuery();

                }
                db.Close();
            }
        }
    }
}
