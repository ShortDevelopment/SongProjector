#nullable enable

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Toolkit.Uwp;
using SongProjector.Preview;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
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
    public async Task<FrameworkElement> GeneratePresentationAsync(RenderContext context)
    {
        var dpi = DisplayInformation.GetForCurrentView().RawDpiX;

        Image image = new();
        image.Source = await RenderTextSlideAsync(_songBeamerFile.Sections[context.SlideId]?.GetContent() ?? "", context.Size, dpi);
        return image;
    }
    #region Render
    public static double OutlineThickness { get; set; } = 10;
    public static Color OutlineColor { get; set; } = Colors.Black;
    public static Color TextColor { get; set; } = Colors.White;
    public static CanvasHorizontalAlignment HorizontalAlignment { get; set; } = CanvasHorizontalAlignment.Center;
    public static CanvasVerticalAlignment VerticalAlignment { get; set; } = CanvasVerticalAlignment.Bottom;
    public static float Margin { get; set; } = 10;

    static CanvasTextLayout GenerateTextLayout(ICanvasResourceCreator resourceCreator, string lineContent, Size space, double desiredFontSize = 300)
    {
        CanvasTextFormat format = new()
        {
            HorizontalAlignment = HorizontalAlignment,
            VerticalAlignment = VerticalAlignment,
            WordWrapping = CanvasWordWrapping.NoWrap,
            FontSize = (float)desiredFontSize,
            FontFamily = "Calibri"
        };

        using (CanvasTextLayout layout = new(resourceCreator, lineContent, format, (float)space.Width, (float)space.Height))
        {
            var drawingSize = layout.DrawBounds.ToSize();

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
            session.Clear(Colors.Green);

            Vector2 position = new(Margin, Margin);
            using (var layout = GenerateTextLayout(session, content, new Size(size.Width - Margin * 2, size.Height - Margin * 2)))
            {
                if (OutlineThickness > 0)
                {
                    using (var geometry = CanvasGeometry.CreateText(layout))
                    using (CanvasStrokeStyle strokeStyle = new())
                        session.DrawGeometry(geometry, position, OutlineColor, (float)OutlineThickness, strokeStyle);
                }
                session.DrawTextLayout(layout, position, TextColor);
            }
            session.Flush();
        }

        InMemoryRandomAccessStream stream = new();
        await renderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png);
        await renderTarget.SaveAsync(@"C:\Users\lukas\Desktop\test.jpg", CanvasBitmapFileFormat.Png);

        BitmapImage result = new();
        result.SetSource(stream);
        return result;
    }
    #endregion

    public async Task<PreviewResult> GeneratePreviewAsync(RenderContext context)
    {
        var content = await GeneratePresentationAsync(context);
        return new()
        {
            Title = _songBeamerFile.Sections[context.SlideId].Title,
            Content = content
        };
    }
}

class SongBeamerFile
{
    public static SongBeamerFile Parse(StorageFile file)
        => Parse(File.ReadAllLines(file.Path));

    public static SongBeamerFile Parse(string[] lines)
    {
        SongBeamerFile result = new();
        Section section = new();
        foreach (var line in lines)
        {
            if (line.StartsWith("#") && line.Contains("="))
            {
                var keyValue = line.Split('=');
                result.Properties.TryAdd(keyValue[0], keyValue[1]);
            }
            else if (line.StartsWith("<"))
            {
                // ToDo
            }
            else if (line == "--" || line == "---")
            {
                bool newSection = line == "---";
                if (section.Lines.Count > 0)
                    result.Sections.Add(section);
                section = new();
            }
            else
            {
                bool foundTitle = false;
                foreach (var keyword in Keywords)
                    if (line.StartsWith(keyword))
                    {
                        section.Title = line;
                        foundTitle = true;
                    }
                if (foundTitle)
                    continue;

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
            result.Sections.Add(section);
        return result;
    }

    static readonly string[] Keywords = new[]
    {
        "Vers",
        "Refrain",
        "Bridge",
        "Chorus",
        "Intro",
        "Outro"
    };

    static readonly Regex ContentLineRegex = new(@"^(#(\d) )?(<(.*):(.+)>)?(.*)$", RegexOptions.Compiled);

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
    public class Section
    {
        public string? Title { get; set; }
        public List<Line> Lines { get; } = new();

        public string GetContent()
            => string.Join("\n", Lines.Select((x) => x.Content));

    }
    public record Line(int LanguageId, string Content);

    public List<Section> Sections { get; } = new();
    #endregion
}
