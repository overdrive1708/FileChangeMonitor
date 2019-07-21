using System.Data.SQLite;

namespace FileChangeMonitor
{
    class CommonFuncClass
    {
        public void CreateDatabase()
        {
            var sqlcon = new SQLiteConnection("Data Source = MonitorFiles.db");
            sqlcon.Open();
            SQLiteCommand createCommand = sqlcon.CreateCommand();
            createCommand.CommandText = "CREATE TABLE IF NOT EXISTS FileInfo(" +
                "Id INTEGER PRIMARY KEY," +
                "FilePath TEXT NOT NULL," +
                "FileTimeStamp TEXT NOT NULL," +
                "Note TEXT)";
            createCommand.ExecuteNonQuery();
            sqlcon.Close();
        }
    }
}
