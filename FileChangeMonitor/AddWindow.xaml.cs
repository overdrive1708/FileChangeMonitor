using System;
using System.Windows;
using System.Data.SQLite;

namespace FileChangeMonitor
{
    /// <summary>
    /// AddWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AddWindow : Window
    {
        public AddWindow()
        {
            InitializeComponent();
            listBoxAddFiles.Items.Add("Please drop files here.");
        }

        private void ListBoxAddFiles_PreviewDragOver(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void ListBoxAddFiles_Drop(object sender, DragEventArgs e)
        {
            listBoxAddFiles.Items.Remove("Please drop files here.");
            if (e.Data.GetData(DataFormats.FileDrop) is string[] dropitems)
            {
                foreach (string dropitem in dropitems)
                {
                    if (System.IO.Directory.Exists(dropitem) == true)
                    {
                        if(System.IO.Directory.GetFiles(@dropitem, "*", System.IO.SearchOption.AllDirectories) is string[] files)
                        {
                            foreach (string file in files)
                            {
                                AddFile(file);
                            }
                        }
                    }
                    else
                    {
                        AddFile(dropitem);
                    }
                }
            }
        }

        private void AddFile(string file)
        {
            if (listBoxAddFiles.Items.Contains(file) == false)
            {
                listBoxAddFiles.Items.Add(file);
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists("MonitorFiles.db") == false)
            {
                CommonFuncClass cmnFunc = new CommonFuncClass();
                cmnFunc.CreateDatabase();
            }
            var sqlcon = new SQLiteConnection("Data Source = MonitorFiles.db");
            sqlcon.Open();
            SQLiteCommand createCommand = sqlcon.CreateCommand();
            createCommand.Transaction = sqlcon.BeginTransaction();
            foreach (string file in listBoxAddFiles.Items)
            {
                createCommand.CommandText = "INSERT INTO FileInfo(Id, FilePath, FileTimeStamp, Note) VALUES(" +
                    "NULL, ?, ?, ?)";
                createCommand.Parameters.Clear();
                var FilePath = new SQLiteParameter { DbType = System.Data.DbType.String, Value = file};
                createCommand.Parameters.Add(FilePath);
                var FileTimeStamp = new SQLiteParameter { DbType = System.Data.DbType.String, Value = System.IO.File.GetLastWriteTime(@file) };
                createCommand.Parameters.Add(FileTimeStamp);
                var Note = new SQLiteParameter { DbType = System.Data.DbType.String, Value = textBoxNote.Text };
                createCommand.Parameters.Add(Note);
                createCommand.Prepare();
                createCommand.ExecuteNonQuery();
            }
            createCommand.Transaction.Commit();
            sqlcon.Close();
            this.Close();
        }
    }
}
