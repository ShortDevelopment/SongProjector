using Microsoft.Office.Core;
using SongProjector.Presentation;
using SongProjector.Preview;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using PptApplication = Microsoft.Office.Interop.PowerPoint.Application;
using PptPresentation = Microsoft.Office.Interop.PowerPoint.Presentation;

namespace SongProjector.Media;

internal class PptMedia : MediaBase, IMedia, IExternalPresentation
{
    PptApplication _app;
    PptPresentation _presentation;
    private PptMedia(StorageFile file, PptApplication app, PptPresentation presentation) : base(file)
    {
        _app = app;
        _presentation = presentation;
    }

    public static async Task<PptMedia> CreateFromFileAsync(StorageFile file)
    {
        TaskCompletionSource<PptMedia> promise = new();
        PptApplication app = new();
        app.Visible = MsoTriState.msoTrue;
        app.AfterPresentationOpen += (PptPresentation pres) =>
        {
            promise.SetResult(new(file, app, pres));
        };
        app.Presentations.Open(file.Path);
        return await promise.Task;
    }

    public override int SlideCount
        => _presentation.Slides.Count;

    public Task<PresentationResult> GeneratePresentationAsync(RenderContext context)
        => throw new NotImplementedException();

    public Task<PreviewResult> GeneratePreviewAsync(RenderContext context)
    {
        var slideId = context.SlideId;
        var tempFilePath = Path.GetTempFileName();
        _presentation.Slides[slideId + 1].Export(tempFilePath, "jpg", 160, 90);

        Image result = new();
        result.Source = new BitmapImage(new Uri(tempFilePath));

        // ToDo: System.IO.File.Delete(tempFilePath);

        return Task.FromResult<PreviewResult>(result);
    }

    public void Show()
    {
        _presentation.SlideShowSettings.ShowPresenterView = MsoTriState.msoFalse;
        _presentation.SlideShowSettings.Run();            
    }

    public void Blank()
        => _presentation.SlideShowWindow.View.Exit();

    public void Next()
        => _presentation.SlideShowWindow.View.Next();

    public void Previous()
        => _presentation.SlideShowWindow.View.Previous();

    public int CurrentSlideIndex
        => _presentation.SlideShowWindow.View.Slide.SlideID - 1;

    public void GoToSlide(int slideId)
        => _presentation.SlideShowWindow.View.GotoSlide(slideId + 1);
}
