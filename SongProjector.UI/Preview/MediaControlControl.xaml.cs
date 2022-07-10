using System;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SongProjector.Preview
{
    public sealed partial class MediaControlControl : UserControl
    {
        public MediaControlControl()
        {
            this.InitializeComponent();
        }

        public event Action Play;
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Play?.Invoke();
        }

        public event Action Pause;
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            Pause?.Invoke();
        }

        public event Action Stop;
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Stop?.Invoke();
        }
    }
}
