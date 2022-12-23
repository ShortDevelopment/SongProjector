#nullable enable

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Toolkit.Uwp;
using SongProjector.Presentation;
using SongProjector.Preview;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace SongProjector.Media;

internal class SongMedia : MediaBase, IMedia
{
    SongBeamerFile _songBeamerFile;
    private SongMedia(StorageFile file, SongBeamerFile songBeamerFile) : base(file)
    {
        _songBeamerFile = songBeamerFile;
    }

    public static Task<SongMedia> CreateFromFileAsync(StorageFile file)
    {
        var content = SongBeamerFile.Parse(file);
        return Task.FromResult<SongMedia>(new(file, content));
    }

    public override int SlideCount
        => _songBeamerFile.Sections.Count;
    public async Task<PresentationResult> GeneratePresentationAsync(RenderContext context)
    {
        var dpi = DisplayInformation.GetForCurrentView().LogicalDpi;

        Image image = new();
        image.Source = await RenderTextSlideAsync(_songBeamerFile.Sections[context.SlideId]?.GetContent() ?? "", context.Size, dpi);
        return image;
    }
    #region Render
    public static float OutlineThicknessScalar { get; set; } = 0.08f;
    public static double DesiredFontSize { get; set; } = 300;
    public static Color OutlineColor { get; set; } = Colors.Black;
    public static Color TextColor { get; set; } = Colors.White;
    public static CanvasHorizontalAlignment HorizontalAlignment { get; set; } = CanvasHorizontalAlignment.Center;
    public static CanvasVerticalAlignment VerticalAlignment { get; set; } = CanvasVerticalAlignment.Bottom;
    public static Vector2 MarginScalar { get; set; } = new(0.05f, 0.05f);

    static CanvasTextLayout GenerateTextLayout(ICanvasResourceCreator resourceCreator, string lineContent, Size space)
    {
        CanvasTextFormat format = new()
        {
            HorizontalAlignment = HorizontalAlignment,
            VerticalAlignment = VerticalAlignment,
            WordWrapping = CanvasWordWrapping.NoWrap,
            FontSize = (float)DesiredFontSize,
            FontFamily = "Inter" // "Assets/Inter-Regular.ttf#Inter"
            // await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Inter-Regular.ttf"));
        };

        using (CanvasTextLayout layout = new(resourceCreator, lineContent, format, (float)space.Width, (float)space.Height))
        {
            var drawingSize = layout.LayoutBoundsIncludingTrailingWhitespace.ToSize();

            var dX = drawingSize.Width / space.Width;
            var dY = drawingSize.Height / space.Height;

            if (dX > 1 || dY > 1)
            {
                if (dX >= dY)
                    format.FontSize /= (float)dX;
                else
                    format.FontSize /= (float)dY;
            }

            return new(resourceCreator, lineContent, format, (float)space.Width, (float)space.Height);
        }
    }

    public static async Task<BitmapImage> RenderTextSlideAsync(string content, Size size, float dpi)
    {
        var device = CanvasDevice.GetSharedDevice();
        CanvasRenderTarget renderTarget = new(device, (float)size.Width, (float)size.Height, dpi);

        var session = renderTarget.CreateDrawingSession();
        {
            session.Clear(Colors.Transparent);

            double mX = size.Width * MarginScalar.X, mY = size.Height * MarginScalar.Y;
            Vector2 position = new((float)mX, (float)mY);
            using (var layout = GenerateTextLayout(session, content, new Size(size.Width - mX * 2, size.Height - mY * 2)))
            {
                if (OutlineThicknessScalar > 0)
                {
                    using (var geometry = CanvasGeometry.CreateText(layout))
                    using (CanvasStrokeStyle strokeStyle = new())
                        session.DrawGeometry(geometry, position, OutlineColor, layout.DefaultFontSize * OutlineThicknessScalar, strokeStyle);
                }
                session.DrawTextLayout(layout, position, TextColor);
            }
            session.Flush();
        }

        InMemoryRandomAccessStream stream = new();
        await renderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png);

        BitmapImage result = new();
        result.SetSource(stream);
        return result;
    }
    #endregion

    public async Task<PreviewResult> GeneratePreviewAsync(RenderContext context)
    {
        var content = await GeneratePresentationAsync(context);
        var section = _songBeamerFile.Sections[context.SlideId];
        return new()
        {
            Title = section.Title,
            TitleColor = section.Color,
            Content = content.Content
        };
    }
}

class SongBeamerFile
{
    public static SongBeamerFile Parse(StorageFile file)
        => Parse(File.ReadAllText(file.Path, Encoding.GetEncoding(1252)).Split("\r\n"));

    public static SongBeamerFile Parse(string[] lines)
    {
        SongBeamerFile result = new();

        List<Section> sections = new();
        Section section = new(result);
        foreach (var line in lines)
        {
            if (line.StartsWith("#") && line.Contains("="))
            {
                var keyValue = line.Split('=');
                result.Properties.TryAdd(keyValue[0].Replace("#", null), keyValue[1]);
            }
            else if (line.StartsWith("--"))
            {
                if (section.Lines.Count > 0)
                    sections.Add(section);

                section = new(result);
                section.IsStandalone = (line == "---");
            }
            else
            {
                var keyword = Keywords.FirstOrDefault(x => line.StartsWith(x.Key));
                if (keyword.Key != null)
                {
                    section.Title = line;
                    section.Color = keyword.Value;
                    continue;
                }
                if (!section.IsStandalone)
                {
                    var last = sections.LastOrDefault();
                    if (last != null)
                    {
                        section.Title = last.Title;
                        section.Color = last.Color;
                    }
                }

                var match = ContentLineRegex.Match(line);
                if (!match.Success)
                    throw new InvalidDataException();

                int langId = 1;
                {
                    var langIdGroup = match.Groups[2];
                    if (langIdGroup.Success)
                        langId = int.Parse(langIdGroup.Value);
                }
                var content = match.Groups[6].Value;
                section.Lines.Add(new(langId, content));
            }
        }
        if (section.Lines.Count > 0)
            sections.Add(section);

        result.AllSections = sections.AsReadOnly();
        result.Sections = result.GetOrderedSections(sections);

        return result;
    }

    static readonly Color VersColor = Color.FromArgb(255, 21, 101, 192);
    static readonly Color SpecialColor = Color.FromArgb(255, 216, 67, 21);
    static readonly Color RepetitiveColor = Color.FromArgb(255, 46, 125, 50);

    static readonly Dictionary<string, Color> Keywords = new()  {
        { "Pre-Chorus", RepetitiveColor },
        { "Pre-Refrain", RepetitiveColor },
        { "Pre-Brige", RepetitiveColor },
        { "Pre-Coda", RepetitiveColor },
        { "Refrain", RepetitiveColor },
        { "Chorus", RepetitiveColor },
        { "Vers", VersColor },
        { "Strophe", VersColor },
        { "Intro", SpecialColor },
        { "Bridge", SpecialColor },
        { "Ending", SpecialColor },
        { "Outro", SpecialColor },
        { "Zwischenspiel", SpecialColor },
        { "Part", SpecialColor },
    };

    static readonly Regex ContentLineRegex = new(@"^(#(\d) )?(<(.*):(.+)>)?(.*)$", RegexOptions.Compiled);

    private SongBeamerFile() { }

    #region Properties
    public Dictionary<string, string> Properties { get; } = new();

    public string? TryGetProp(string key)
        => TryGetProp<string>(key);

    public T? TryGetProp<T>(string key)
    {
        if (Properties.TryGetValue(key, out var value))
            return (T)Convert.ChangeType(value, typeof(T));
        return default(T);
    }

    public string? Title => TryGetProp("Title");
    public string? Author => TryGetProp("Author");
    #endregion

    #region Content
    public sealed class Section
    {
        readonly SongBeamerFile _file;
        public Section(SongBeamerFile file)
            => _file = file;

        public string? Title { get; set; }
        public Color? Color { get; set; }
        public bool IsStandalone { get; set; } = true;

        public List<Line> Lines { get; } = new();

        public string GetContent(int langId = 1)
        {
            var langCount = _file.TryGetProp<int>("LangCount");
            if (langCount < 1)
                langCount = 1;

            return string.Join("\n", Lines.Where((x, i) => i % langCount == langId - 1).Select((x) => x.Content));
        }

    }
    public record Line(int LanguageId, string Content);

    [AllowNull]
    public ReadOnlyCollection<Section> AllSections { get; private set; }

    [AllowNull]
    public ReadOnlyCollection<Section> Sections { get; private set; }

    ReadOnlyCollection<Section> GetOrderedSections(List<Section> sections)
    {
        var verseOrder = TryGetProp("VerseOrder");
        if (verseOrder != null)
        {
            List<Section> result = new();
            foreach (var verse in verseOrder.Split(','))
                result.AddRange(sections.Where(x => x.Title == verse));

            if (result.Count != 0)
                return result.AsReadOnly();
        }
        return sections.AsReadOnly();
    }
    #endregion
}
