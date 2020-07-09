using System.Windows.Controls;

namespace arcoreimg_app.Controls
{
    /// <summary>
    /// Interaction logic for AsProgress.xaml
    /// </summary>
    public partial class AsProgress : UserControl
    {
        public AsProgress()
        {
            InitializeComponent();
        }

        public string Percentage
        {
            get { return TxtProgress.Text; }
            set { TxtProgress.Text = value; }
        }

        public string Ongoing
        {
            get { return TxtTask.Text; }
            set { TxtTask.Text = value; }
        }

        public int Value
        {
            get { return (int)LoadingBar.Value; }
            set { LoadingBar.Value = value; }
        }

        public bool IsIndeterminate
        {
            get { return LoadingBar.IsIndeterminate; }
            set { LoadingBar.IsIndeterminate = value; }
        }
    }
}
