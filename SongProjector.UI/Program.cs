using Windows.UI.Xaml;

namespace SongProjector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FullTrustApplication.Start(_ => new App(), new("SongProjector"));
        }
    }
}
