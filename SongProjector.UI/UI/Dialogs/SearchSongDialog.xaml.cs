using Windows.UI.Xaml.Controls;

namespace SongProjector.UI.Dialogs;

public sealed partial class SearchSongDialog : ContentDialog
{
    public SearchSongDialog()
    {
        this.InitializeComponent();
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {

    }

    private void SearchTextBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {

    }
}
