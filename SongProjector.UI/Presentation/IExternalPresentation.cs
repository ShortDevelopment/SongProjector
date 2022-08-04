namespace SongProjector.Presentation
{
    public interface IExternalPresentation
    {
        void Show();
        void Blank();

        void Next();
        void Previous();

        int CurrentSlideIndex { get; }
        void GoToSlide(int slideId);
    }
}
