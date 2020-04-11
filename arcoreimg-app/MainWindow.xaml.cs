using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace arcoreimg_app
{
    /// <summary>
    /// Interaction logic for MaarciViewer.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string ImgFilename, DirFilename, DirDatabase, ARCoreImg, NewDatabase;
        private long _filesize;

        public MainWindow()
        {
            InitializeComponent();
            ARCoreImg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "arcoreimg.exe");
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
                ImageUri.Text = ImgFilename = dlg.FileName;
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
            _filesize = new FileInfo(ImgFilename).Length;
            int fsize = int.Parse(_filesize.ToString()) / 1000000;
            TxtFilename.Text = Path.GetFileName(ImgFilename);
            Process process = CreateProcess("/C \"arcoreimg.exe eval-img --input_image_path=" + ImgFilename);
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
                DirFilename = dlgLst.FileName;
            
                CommonOpenFileDialog dlgDb = new CommonOpenFileDialog()
                {
                    Title = "Select Where to save the Image Database",
                    InitialDirectory = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Documents"),
                    IsFolderPicker = true
                };

                if (dlgDb.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    DirDatabase = dlgDb.FileName;
                    NewDatabase = "myimages_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".imgdb";
                    Process process = CreateProcess("/C \"arcoreimg.exe build-db --input_images_directory=" + DirFilename +
                        " --output_db_path=" + DirDatabase + "/" + NewDatabase);
                    process.Start();

                    try
                    {
                        string result = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();//NewDatabase, LstFilename
                        TxtFeedback1.Text = "A new Database: " + NewDatabase + " from an Image directory: " + DirFilename +
                            "\n\nYou can specify another directory that contains your images to create another database.";
                    }
                    catch (Exception ex)
                    {
                        //arcoreimg.WriteLogs("App Errors", @" " + ex.Message, @"" + ex.InnerException, @"" + ex.StackTrace);
                    }
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
                DirFilename = dlgLst.FileName;

                CommonOpenFileDialog dlgDb = new CommonOpenFileDialog()
                {
                    Title = "Select Where to save the Image Database",
                    InitialDirectory = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Documents"),
                    IsFolderPicker = true
                };

                if (dlgDb.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    DirDatabase = dlgDb.FileName;
                    NewDatabase = "myimages_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".imgdb";

                    Process process = CreateProcess("/C \"arcoreimg.exe build-db --input_image_list_path=" + DirFilename +
                        " --output_db_path=" + DirDatabase + "/" + NewDatabase);
                    process.Start();

                    try
                    {
                        string result = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();//NewDatabase, LstFilename
                        TxtFeedback2.Text = "A new database: " + NewDatabase + " from an image list file: " + DirFilename +
                            "\n\nBrowse to another Image File List to Create a Database From";
                    }
                    catch (Exception ex)
                    {
                        //arcoreimg.WriteLogs("App Errors", @" " + ex.Message, @"" + ex.InnerException, @"" + ex.StackTrace);
                    }
                }

            }
        }

    }
}