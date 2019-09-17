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
VALUES 
    ('superadmin'),
    ('bookreader'),
    ('bookwriter');", db))
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
                }
                db.Close();
            }
        }
    }
}
