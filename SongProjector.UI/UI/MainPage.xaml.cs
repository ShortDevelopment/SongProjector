#nullable enable

using SongProjector.Media;
using SongProjector.Presentation;
using SongProjector.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinUI.Interop.CoreWindow;

namespace SongProjector.UI;

public sealed partial class MainPage : Page
{
    ObservableCollection<IMedia> scheduleCollection = new();
    public MainPage()
    {
        this.InitializeComponent();
        ScheduleListView.ItemsSource = scheduleCollection;
    }

    internal static List<PresentationManager> PresentationManagers { get; } = new();

    #region Command Bar
    private async void OpenSearchButton_Click(object sender, RoutedEventArgs e)
    {
        SearchSongDialog dialog = new();
        await dialog.ShowAsync();
    }

    private async void StartPresentationButton_Click(object sender, RoutedEventArgs e)
    {
        if (PresentationManagers.Count == 0)
        {
            var manager = await PresentationManager.CreateForScreenAsync(1);
            manager.Background = Colors.Green;
            PresentationManagers.Add(manager);
            // PresentationManagers.Add(await PresentationManager.CreateForScreenAsync(2));
        }
        PreviewPage.DeselectPreviewItem();
        PresentationManagers.ForEach(p => p.Start());

        StartPresentationButton.IsEnabled = false;
        StopPresentationButton.IsEnabled = true;
    }

    private void BlankButton_Click(object sender, RoutedEventArgs e)
    {
        PresentationManagers.ForEach(p => p.Presentation?.Blank(PreviewPage.CurrentMedia));
        PreviewPage.DeselectPreviewItem();
    }

    private void StopPresentationButton_Click(object sender, RoutedEventArgs e)
    {
        PresentationManagers.ForEach(p => p.Stop());
        PreviewPage.DeselectPreviewItem();

        StartPresentationButton.IsEnabled = true;
        StopPresentationButton.IsEnabled = false;
    }
    #endregion

    #region MenuBar
    private void ExitMenuButton_Click(object sender, RoutedEventArgs e)
    {
        Window.Current.Close();
    }

    #region Insert
    void InsertMedia(IMedia media)
    {
        scheduleCollection.Add(media);
        ScheduleListView.SelectedItem = media;
    }

    private async void InsertPdfMenuButton_Click(object sender, RoutedEventArgs e)
    {
        var file = await PickFileAsync(".pdf");
        if (file != null)
        {
            IMedia media = await PdfFileMedia.CreateFromFileAsync(file);
            InsertMedia(media);
        }
    }

    private async void InsertPptMenuButton_Click(object sender, RoutedEventArgs e)
    {
        var file = await PickFileAsync(".pptx");
        if (file != null)
        {
            IMedia media = await PptMedia.CreateFromFileAsync(file);
            InsertMedia(media);
        }
    }

    private async void InsertImageButton_Click(object sender, RoutedEventArgs e)
    {
        var file = await PickFileAsync(new[] { ".jpg", ".jpeg", ".png" });
        if (file != null)
        {
            IMedia media = await ImageMedia.CreateFromFileAsync(file);
            InsertMedia(media);
        }
    }

    private async void InsertVideoButton_Click(object sender, RoutedEventArgs e)
    {
        var file = await PickFileAsync(new[] { ".mov", ".mp4" });
        if (file != null)
        {
            IMedia media = await VideoMedia.CreateFromFileAsync(file);
            InsertMedia(media);
        }
    }

    private void InserTextButton_Click(object sender, RoutedEventArgs e)
    {
        IMedia media = new TextMedia();
        InsertMedia(media);
    }

    private async void InsertSongButton_Click(object sender, RoutedEventArgs e)
    {
        var file = await PickFileAsync(new[] { ".sng", ".txt" });
        if (file != null)
        {
            IMedia media = await SongMedia.CreateFromFileAsync(file);
            InsertMedia(media);
        }
    }
    #endregion
    #endregion

    async Task<StorageFile> PickFileAsync(string filter)
        => await PickFileAsync(new[] { filter });
    async Task<StorageFile> PickFileAsync(string[] filters)
    {
        FileOpenPicker openPicker = new();
        ((IInitializeWithWindow)(object)openPicker).Initialize(CoreWindowInterop.CoreWindowHwnd);
        openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        foreach (var filter in filters)
            openPicker.FileTypeFilter.Add(filter);
        return await openPicker.PickSingleFileAsync();
    }

    private void ScheduleListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        PreviewPage.CurrentMedia = ScheduleListView.SelectedItem as IMedia;
    }
}