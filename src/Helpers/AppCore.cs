using System;
using System.Diagnostics;
using System.IO;

namespace arcoreimg_app.Helpers
{
    public class AppCore
    {
        /// <summary>
        /// Create a cmd Process
        /// </summary>
        /// <param name="args">args</param>
        /// <returns></returns>

        public static Process CreateProcess(string args)
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

        public static string GetFileSize(double len)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }

        public static AsScanned CheckImage(string imagePath)
        {
            AsScanned scan = new AsScanned();
            scan.Image = imagePath;

            double filelen = new FileInfo(imagePath).Length;
            string f_size = " (" + AppCore.GetFileSize(filelen) + ")";
            Process process = CreateProcess("/C \"arcoreimg.exe eval-img --input_image_path=" + imagePath);
            process.Start();

            try
            {
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                int score = int.Parse(result);
                scan.Score = score;


                if (score < 49)
                    scan.Title = Path.GetFileName(imagePath) + f_size + " | Poor Quality Image";

                else if (score > 50 && score < 89)
                    scan.Title = Path.GetFileName(imagePath) + f_size + " | Good Quality Image";

                else if (score > 90)
                    scan.Title = Path.GetFileName(imagePath) + f_size + " | Best Quality Image";
            }
            catch (Exception)
            {
                //arcoreimg.WriteLogs("App Errors", @" " + ex.Message, @"" + ex.InnerException, @"" + ex.StackTrace);
            }
            return scan;
        }
        public static string Todate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        public static string JustNow()
        {
            return DateTime.Now.ToString("HH:mm:ss tt");
        }
        public static string TimeStamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }

        public static void WriteLogs(string source, string message, string innerexception, string stacktrace)
        {
            string FileLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppCore.Todate() + "-logs.log");
            using (StreamWriter sw = new StreamWriter(FileLog, true))
            {
                sw.WriteLine("maarcilog>");
                sw.WriteLine("    <time>" + JustNow() + "</time>");
                sw.WriteLine("    <source>" + source + "</source>");
                sw.WriteLine("    <message>" + message + "</message>");
                if (innerexception.Length > 0) sw.WriteLine("    <innerexception>" + innerexception + "</innerexception>");
                if (stacktrace.Length > 0) sw.WriteLine("    <stacktrace>" + stacktrace + "</stacktrace>");
                sw.WriteLine("</maarcilog>");
                sw.WriteLine("");
                sw.Dispose();
            }
        }
        /// <summary>
        /// this is the path to the root directory of maarci's content directory \appdata\local\MaarciApp
        /// </summary>

        public static string Local
        {
            get
            {
                string rootDir = Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString();
                return Path.Combine(rootDir, "Apps", "Malezi", "MA-ARCoreImg");
            }
        }

        /// <summary>
        /// this is the path to the logs directory \appdata\local\MaarciApp\logs
        /// </summary>
        public static string Logs
        {
            get
            {
                return Path.Combine(Local, "logs");
            }
        }

        /// <summary>
        /// this is the path to the daily log file in logs directory 
        /// \appdata\local\MaarciApp\logs\Todate() + "-logs.log"
        /// </summary>
        public static string FileLog
        {
            get
            {
                return Path.Combine(Logs, AppCore.Todate() + "-logs.log");
            }
        }

        /// <summary>
        /// this is the path to the daily log file in logs directory 
        /// \appdata\local\MaarciApp\logs\Processlogs.log"
        /// </summary>
        public static string JustNowLog
        {
            get
            {
                return Path.Combine(Logs, "JustNow.log");
            }
        }

        public static string MaarciData
        {
            get
            {
                return Path.Combine(Local, "myimages.imgdb");
            }
        }

    }

}
