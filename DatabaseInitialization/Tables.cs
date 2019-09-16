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
    book_checkedIn INTEGER,
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
    }
}
