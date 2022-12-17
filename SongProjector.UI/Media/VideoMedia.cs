using SongProjector.Preview;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SongProjector.Media
{
    internal class VideoMedia : MediaBase, IMedia
    {
        public VideoMedia(StorageFile file) : base(file) { }

        public static Task<VideoMedia> CreateFromFileAsync(StorageFile file)
            => Task.FromResult(new VideoMedia(file));

        public override int SlideCount
            => 1;

        List<DispatchableReference<MediaPlayer>> playerRefs = new();
        public Task<FrameworkElement> GeneratePresentationAsync(RenderContext context)
        {
            if (context.SlideId != 0)
                throw new ArgumentOutOfRangeException();

            MediaPlayerElement playerEle = new();
            playerEle.Source = MediaSource.CreateFromStorageFile(File);

            playerRefs.Add(DispatchableReference<MediaPlayer>.Create(playerEle.MediaPlayer, Window.Current.Dispatcher));

            return Task.FromResult<FrameworkElement>(playerEle);
        }

        public Task<PreviewResult> GeneratePreviewAsync(RenderContext context)
        {
            if (context.SlideId != 0)
                throw new ArgumentOutOfRangeException();

            MediaControlControl control = new();
            control.Play += () =>
            {
                foreach (var playerRef in playerRefs)
                    playerRef.Do((player) => player.Play());
            };
            control.Pause += () =>
            {
                foreach (var playerRef in playerRefs)
                    playerRef.Do((player) => player.Pause());
            };
            control.Stop += () =>
            {
                foreach (var playerRef in playerRefs)
                    playerRef.Do((player) =>
                    {
                        player.Pause();
                        player.PlaybackSession.Position = TimeSpan.FromMilliseconds(0);
                    });
            };
            return Task.FromResult<PreviewResult>(control);
        }
    }
}
