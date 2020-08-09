using SQLite;

namespace ReminderApp.Services.SQLiteService
{
    public interface IDatabaseConnection
    {
        SQLiteConnection SqliteConnection(string databaseName);
        long GetSize(string databaseName);
        string GetDatabasePath();
    }
}
