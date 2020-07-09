using arcoreimg_app.Controls;
using arcoreimg_app.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Documents;

namespace arcoreimg_app
{
    public class AppTask
    {
        string _dirpath;
        BackgroundWorker _worker;
        List<AsScanned> _scans = new List<AsScanned>();

        public AppTask(string dirpath)
        {
            _dirpath = dirpath;
            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = false;
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += DoWork;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (string file in Directory.EnumerateFiles(_dirpath, "*.*",
                    SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg")))
            {
                AsScanned asListItem = AppCore.CheckImage(file);
                _scans.Add(asListItem);
            }
            e.Result = _scans;
        }

        public event ProgressChangedEventHandler ProgressChanged
        {
            add { _worker.ProgressChanged += value; }
            remove { _worker.ProgressChanged -= value; }
        }

        public event RunWorkerCompletedEventHandler Completed
        {
            add { _worker.RunWorkerCompleted += value; }
            remove { _worker.RunWorkerCompleted -= value; }

        }

        public void StartAsync()
        {
            _worker.RunWorkerAsync();
        }

        public void Dispose()
        {
            _worker.Dispose();
        }
    }
}
