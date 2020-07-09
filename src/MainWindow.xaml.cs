using arcoreimg_app.Controls;
using arcoreimg_app.Helpers;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace arcoreimg_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string FileListPath, NewDatabaseNamePath, NewDatabaseName;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtDirPath1.Text = TxtDirPath3.Text = NewDatabaseNamePath = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Documents");
            TxtDirPath2.Text = TxtDirPath4.Text = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Downloads");
        }

        private void BtnImgBrowser_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select an Image",
                Filter = "Images |*.jpg; *.png",
                CheckFileExists = true
            };
            if (dlg.ShowDialog() == true)
            {
                AsScanned scan = AppCore.CheckImage(dlg.FileName);
                AsListItem asListItem = new AsListItem
                {
                    Title = scan.Title,
                    Image = scan.Image,
                    Score = scan.Score
                };
                ImageList.Children.Add(asListItem);
            }
        }

        private void BtnDirBrowser_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dlgLst = new CommonOpenFileDialog()
            {
                Title = "Select an Image Directory",
                InitialDirectory = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Downloads"),
                IsFolderPicker = true
            };

            if (dlgLst.ShowDialog() == CommonFileDialogResult.Ok)
            {
                LoadingBar.Visibility = Visibility.Visible;
                AppTask arcoreimg_appTask = new AppTask(dlgLst.FileName);
                arcoreimg_appTask.StartAsync();
                arcoreimg_appTask.ProgressChanged += AppTask_ProgressChanged;
                arcoreimg_appTask.Completed += AppTask_Completed;
            }
        }

        private void AppTask_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            LoadingBar.Visibility = Visibility.Visible;
        }

        private void AppTask_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.Dispose();
            List<AsScanned> scanned = (List<AsScanned>)e.Result;

            for (int i = 0; i < scanned.Count; i++)
            {
                AsScanned scan = scanned[i];
                AsListItem asListItem = new AsListItem
                {
                    Title = scan.Title,
                    Image = scan.Image,
                    Score = scan.Score
                };
                ImageList.Children.Add(asListItem);
            }
            LoadingBar.Visibility = Visibility.Collapsed;
        }


        private void BtnDbDirBrowser_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dlgDb = new CommonOpenFileDialog()
            {
                Title = "Select Where to save the Image Database",
                InitialDirectory = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Documents"),
                IsFolderPicker = true
            };
            if (dlgDb.ShowDialog() == CommonFileDialogResult.Ok)
            {
                TxtDirPath1.Text = TxtDirPath3.Text = NewDatabaseNamePath = dlgDb.FileName;
            }
        }

        private void BtnImgDirBrowser_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dlgLst = new CommonOpenFileDialog()
            {
                Title = "Select an Image Directory",
                InitialDirectory = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Downloads"),
                IsFolderPicker = true
            };

            if (dlgLst.ShowDialog() == CommonFileDialogResult.Ok)
            {
                TxtDirPath2.Text = FileListPath = dlgLst.FileName;

                NewDatabaseName = "myimages_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".imgdb";
                Process process = AppCore.CreateProcess($"/C \"arcoreimg.exe build-db --input_images_directory=\"{FileListPath}\" --output_db_path=\"{Path.Combine(NewDatabaseNamePath, NewDatabaseName)}\"");
                process.Start();

                try
                {
                    string result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    TxtFeedback1.Text = "New database: '" + NewDatabaseName + "' created successfully!\nYou may specify another image directory to create another database.";
                }
                catch (Exception)
                {
                    //arcoreimg.WriteLogs("App Errors", @" " + ex.Message, @"" + ex.InnerException, @"" + ex.StackTrace);
                }
            }

        }

        private void BtnTxtBrowser_Click(object sender, RoutedEventArgs e)
        {
            var dlgLst = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select a File List",
                Filter = "File List |*.txt",
                CheckFileExists = true
            };

            if (dlgLst.ShowDialog() == true)
            {
                TxtDirPath4.Text = FileListPath = dlgLst.FileName;

                NewDatabaseName = "myimages_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".imgdb";

                Process process = AppCore.CreateProcess("/C \"arcoreimg.exe build-db --input_image_list_path=" + FileListPath +
                    " --output_db_path=" + NewDatabaseNamePath + "/" + NewDatabaseName);
                process.Start();

                try
                {
                    string result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();//NewDatabaseName, LstFilename
                    TxtFeedback2.Text = "New database: '" + NewDatabaseName + "' created successfully!\nYou may browse another Image File List to create another Database";
                }
                catch (Exception)
                {
                    //arcoreimg.WriteLogs("App Errors", @" " + ex.Message, @"" + ex.InnerException, @"" + ex.StackTrace);
                }
            }
        }

    }
}