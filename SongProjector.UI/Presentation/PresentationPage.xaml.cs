﻿using ShortDev.Uwp.FullTrust.Xaml;
using SongProjector.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SongProjector.Presentation;

public sealed partial class PresentationPage : Page, IPresentation
{
    public PresentationPage()
    {
        this.InitializeComponent();
    }

    public void Present(IMedia media, int slideIndex)
    {
        _ = Dispatcher.RunIdleAsync(async (x) =>
        {
            if (media is IExternalPresentation externalPresenation)
            {
                ClearContent();
                Window.Current.GetSubclass().Win32Window.IsTopMost = false;
                externalPresenation.Show();
                externalPresenation.GoToSlide(slideIndex);
            }
            else
            {
                Window.Current.GetSubclass().Win32Window.IsTopMost = true;
                var result = await media.GeneratePresentationAsync(new()
                {
                    SlideId = slideIndex,
                    Size = new(RenderContainer.ActualWidth, RenderContainer.ActualHeight)
                });
                RenderContainer.Content = result.Content;
            }
        });
    }

    public void Blank(IMedia media)
    {
        ClearContent();
        Window.Current.GetSubclass().Win32Window.IsTopMost = true;
        if (media is IExternalPresentation externalPresenation)
            externalPresenation.Blank();
    }

    private void ClearContent()
    {
        _ = Dispatcher.RunIdleAsync((x) =>
        {
            RenderContainer.Content = null;
        });
    }
}
