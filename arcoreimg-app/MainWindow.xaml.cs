using Microsoft.WindowsAPICodePack.Dialogs;
using System;
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
        string SingleImagePath, FileListPath, NewDatabaseNamePath, NewDatabaseName;
        private long _filesize;

        public MainWindow()
        {
            InitializeComponent();
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
                ImageUri.Text = SingleImagePath = dlg.FileName;
                CheckImage();
            }
        }
        
        /// <summary>
        /// Create a cmd Process
        /// </summary>
        /// <param name="args">args</param>
        /// <returns></returns>

        private Process CreateProcess(string args)
        {
            ProcessStartInfo procstartInfo = new ProcessStartInfo();
            Process process = new Process();
            procstartInfo.FileName = "Cmd.exe";
            procstartInfo.Arguments = args;
            // Do not show the black cmd.
            procstartInfo.CreateNoWindow = true;
            process.StartInfo = procstartInfo;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            return process;
        }


        private void CheckImage()
        {
            _filesize = new FileInfo(SingleImagePath).Length;
            int fsize = int.Parse(_filesize.ToString()) / 1000000;
            TxtFilename.Text = Path.GetFileName(SingleImagePath);
            Process process = CreateProcess("/C \"arcoreimg.exe eval-img --input_image_path=" + SingleImagePath);
            process.Start();

            try
            {
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                int score = int.Parse(result);
                LoadingBar.Value = score;
                string ResultStr = result.Trim() + " %";
                TxtProgress.Text = ResultStr.Trim();

                string f_size = "";
                if (fsize < 100000) f_size = fsize + " MB";
                else if (fsize > 100000) f_size = fsize + " GB";
                else f_size = fsize + " KB";

                switch (score)
                {
                    case 50:
                        TxtFeedback.Text = "Image score average!";
                        break;

                    case 75:
                        TxtFeedback.Text = "Image score above average!";
                        break;

                    case 100:
                        TxtFeedback.Text = "Image passed test!";
                        break;

                    default:
                        TxtFeedback.Text = "Image score poor!";
                        break;
                }
            }
            catch (Exception ex)
            {
                //arcoreimg.WriteLogs("App Errors", @" " + ex.Message, @"" + ex.InnerException, @"" + ex.StackTrace);
            }
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
                Process process = CreateProcess($"/C \"arcoreimg.exe build-db --input_images_directory=\"{FileListPath}\" --output_db_path=\"{Path.Combine(NewDatabaseNamePath, NewDatabaseName)}\"");
                process.Start();

                try
                {
                    string result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    TxtFeedback1.Text = "New database: '" + NewDatabaseName + "' created successfully!\nYou may specify another image directory to create another database.";
                }
                catch (Exception ex)
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

                Process process = CreateProcess("/C \"arcoreimg.exe build-db --input_image_list_path=" + FileListPath +
                    " --output_db_path=" + NewDatabaseNamePath + "/" + NewDatabaseName);
                process.Start();

                try
                {
                    string result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();//NewDatabaseName, LstFilename
                    TxtFeedback2.Text = "New database: '" + NewDatabaseName + "' created successfully!\nYou may browse another Image File List to create another Database";
                }
                catch (Exception ex)
                {
                    //arcoreimg.WriteLogs("App Errors", @" " + ex.Message, @"" + ex.InnerException, @"" + ex.StackTrace);
                }
            }
        }

    }
}