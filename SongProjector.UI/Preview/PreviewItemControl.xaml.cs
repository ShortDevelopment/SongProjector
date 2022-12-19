using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SongProjector.Preview
{
    public sealed partial class PreviewItemControl : UserControl
    {
        public PreviewItemControl()
        {
            this.InitializeComponent();
        }

        public int Index { get; set; }

        public FrameworkElement PreviewContent
        {
            get => (FrameworkElement)PreviewPresenter.Content;
            set => PreviewPresenter.Content = value;
        }

        public string Title
        {
            get => TitlePreviewTextBlock.Text;
            set => TitlePreviewTextBlock.Text = value;
        }

        static readonly Color DefaultTitleColor = Color.FromArgb(255, 0x33, 0x33, 0x33);
        static readonly SolidColorBrush DefaultTitleBrush = new(DefaultTitleColor);

        public static DependencyProperty TitleColorProperty = DependencyProperty.Register(nameof(TitleBrush), typeof(SolidColorBrush), typeof(PreviewItemControl), new PropertyMetadata(DefaultTitleBrush));
        public SolidColorBrush TitleBrush
        {
            get => (SolidColorBrush)GetValue(TitleColorProperty);
            set => SetValue(TitleColorProperty, value ?? DefaultTitleBrush);
        }

        public Color? TitleColor
        {
            get => TitleBrush.Color;
            set => TitleBrush = new(value ?? DefaultTitleColor);
        }
    }
}
