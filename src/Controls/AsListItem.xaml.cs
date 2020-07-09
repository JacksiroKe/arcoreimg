using System.Windows;
using System.Windows.Controls;

namespace arcoreimg_app.Controls
{
    /// <summary>
    /// Interaction logic for ListItem.xaml
    /// </summary>
    public partial class AsListItem : UserControl
    {
        public AsListItem()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string Image
        {
            get { return ImageUri.Text; }
            set { ImageUri.Text = value; }
        }

        public string Title
        {
            get { return ImgTitle.Text; }
            set { ImgTitle.Text = value; }
        }

        public int Score
        {
            get { return int.Parse(ImgScore.Text); }
            set
            {
                ImgScore.Text = value + " %";
                LoadingBar.Value = value;
            }
        }

        public string Itemid { get; internal set; }

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TabItem));

        void RaiseClickEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(AsListItem.ClickEvent);
            RaiseEvent(newEventArgs);
        }

        void OnClick()
        {
            RaiseClickEvent();
        }

    }
}
