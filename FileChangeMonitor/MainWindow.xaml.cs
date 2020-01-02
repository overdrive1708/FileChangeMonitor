using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace FileChangeMonitor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly ObservableCollection<ViewData> ViewCollection = new ObservableCollection<ViewData>();

        public class ViewData
        {
            public string Status { get; set; }
            public int Id { get; set; }
            public string FileName { get; set; }
            public string FileDir { get; set; }
            public string TimeStamp { get; set; }
            public string Note { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            dataGrid.DataContext = ViewCollection;
            ViewDataGrid();
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddWindow
            {
                Owner = GetWindow(this)
            };
            window.ShowDialog();
            ViewDataGrid();
        }

        private void ViewDataGrid()
        {
            ViewCollection.Clear();
            if (System.IO.File.Exists("MonitorFiles.db") == true)
            {
                var sqlcon = new SQLiteConnection("Data Source = MonitorFiles.db");
                sqlcon.Open();
                SQLiteCommand createCommand = sqlcon.CreateCommand();
                createCommand.CommandText = "SELECT * FROM FileInfo";
                SQLiteDataReader sdr = createCommand.ExecuteReader();
                while (sdr.Read() == true)
                {
                    ViewData view = new ViewData
                    {
                        Status = JudgeStatusFileMonitor(sdr["FilePath"].ToString(), sdr["FileTimeStamp"].ToString()),
                        Id = int.Parse(sdr["Id"].ToString()),
                        FileName = System.IO.Path.GetFileName(sdr["FilePath"].ToString()),
                        FileDir = System.IO.Path.GetDirectoryName(sdr["FilePath"].ToString()),
                        TimeStamp = sdr["FileTimeStamp"].ToString(),
                        Note = sdr["Note"].ToString()
                    };
                    ViewCollection.Add(view);
                }
                sdr.Close();
                sqlcon.Close();
                labelNotification.Content = "Notification：Check is successful.";
            }
            else
            {
                labelNotification.Content = "Notification：Database file not found.";
            }
        }

        private string JudgeStatusFileMonitor(string fileName, string fileTimestamp)
        {
            string ret;
            if (System.IO.File.Exists(fileName) == true)
            {
                if(System.IO.File.GetLastWriteTime(fileName).ToString() == fileTimestamp)
                {
                    ret = "OK";
                }
                else
                {
                    ret = "NG：Timestamp is unmatch.";
                }
            }
            else
            {
                ret = "NG：File not found.";
            }
            return (ret);
        }

        private void ButtonReCheck_Click(object sender, RoutedEventArgs e)
        {
            ViewDataGrid();
        }

        private void ButtonTimeStampUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists("MonitorFiles.db") == true)
            {
                List<object> items = dataGrid.SelectedItems.Cast<object>().ToList();
                if (items.Count != 0)
                {
                    foreach (object item in items)
                    {
                        ViewData data = (ViewData)item;
                        string selectedId = data.Id.ToString();
                        string selectedFileName = data.FileName.ToString();
                        string selectedFileDir = data.FileDir.ToString();
                        string selectedFile = System.IO.Path.Combine(selectedFileDir, selectedFileName);
                        if (System.IO.File.Exists(selectedFile) == true)
                        {
                            var sqlcon = new SQLiteConnection("Data Source = MonitorFiles.db");
                            sqlcon.Open();
                            SQLiteCommand createCommand = sqlcon.CreateCommand();
                            createCommand.CommandText = "UPDATE FileInfo SET FileTimeStamp = ? where Id = ?";
                            createCommand.Parameters.Clear();
                            var FileTimeStamp = new SQLiteParameter { DbType = System.Data.DbType.String, Value = System.IO.File.GetLastWriteTime(selectedFile) };
                            createCommand.Parameters.Add(FileTimeStamp);
                            var Id = new SQLiteParameter { DbType = System.Data.DbType.Int32, Value = selectedId };
                            createCommand.Parameters.Add(Id);
                            createCommand.Prepare();
                            createCommand.ExecuteNonQuery();
                            sqlcon.Close();
                            labelNotification.Content = "Notification：TimeStamp update is successful.";
                            ViewDataGrid();
                        }
                        else
                        {
                            labelNotification.Content = "Notification：File not found.";
                        }
                    }
                }
                else
                {
                    labelNotification.Content = "Notification：Not selected.";
                }
            }
            else
            {
                labelNotification.Content = "Notification：Database file not found.";
            }
        }

        private void ButtonDelSelected_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this?", "Caution", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                if (System.IO.File.Exists("MonitorFiles.db") == true)
                {
                    List<object> items = dataGrid.SelectedItems.Cast<object>().ToList();
                    if (items.Count != 0)
                    {
                        foreach (object item in items)
                        {
                            ViewData data = (ViewData)item;
                            string selectedId = data.Id.ToString();
                            var sqlcon = new SQLiteConnection("Data Source = MonitorFiles.db");
                            sqlcon.Open();
                            SQLiteCommand createCommand = sqlcon.CreateCommand();
                            createCommand.CommandText = "DELETE FROM FileInfo where Id = ?";
                            createCommand.Parameters.Clear();
                            var Id = new SQLiteParameter { DbType = System.Data.DbType.Int32, Value = selectedId };
                            createCommand.Parameters.Add(Id);
                            createCommand.Prepare();
                            createCommand.ExecuteNonQuery();
                            sqlcon.Close();
                            labelNotification.Content = "Notification：Delete selected item is successful.";
                            ViewDataGrid();
                        }
                    }
                    else
                    {
                        labelNotification.Content = "Notification：Not selected.";
                    }
                }
                else
                {
                    labelNotification.Content = "Notification：Database file not found.";
                }
            }
            else
            {
                labelNotification.Content = "Notification：Operation cancelled.";
            }
        }

        private void ButtonDelAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this?", "Caution", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                if (System.IO.File.Exists("MonitorFiles.db") == true)
                {
                    var sqlcon = new SQLiteConnection("Data Source = MonitorFiles.db");
                    sqlcon.Open();
                    SQLiteCommand createCommand = sqlcon.CreateCommand();
                    createCommand.CommandText = "DELETE FROM FileInfo";
                    createCommand.ExecuteNonQuery();
                    sqlcon.Close();
                    labelNotification.Content = "Notification：Delete all item is successful.";
                    ViewDataGrid();
                }
                else
                {
                    labelNotification.Content = "Notification：Database file not found.";
                }
            }
            else
            {
                labelNotification.Content = "Notification：Operation cancelled.";
            }
        }
    }
}
