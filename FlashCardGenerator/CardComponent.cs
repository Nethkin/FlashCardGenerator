using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace FlashCardGenerator
{
    internal class CardComponent : IComponent
    {
        #region Private Properties

        private string FontFamily { get; }
        private string Text { get; }

        #endregion

        #region Constructors

        public CardComponent(string text, string fontFamily)
        {
            FontFamily = fontFamily;
            Text = text;
        }

        #endregion

        #region Public Methods

        public void Compose(IContainer container)
        {
            container
            .Layers(layers =>
            {
                layers.Layer().SkiaSharpCanvas((canvas, size) =>
                {
                    DrawRoundedRectangle(Colors.Black, true);

                    void DrawRoundedRectangle(string color, bool isStroke)
                    {
                        using var paint = new SKPaint
                        {
                            Color = SKColor.Parse(color),
                            IsStroke = isStroke,
                            StrokeWidth = 2,
                            IsAntialias = true
                        };
                        canvas.DrawRoundRect(0, 0, size.Width, size.Height, 20, 20, paint);
                    }
                });

                layers
                    .PrimaryLayer()
                    .AlignCenter()
                    .AlignMiddle()
                    .Padding(.125f, Unit.Inch)
                    .Column(column =>
                    {
                        column.Item()
                                .ScaleToFit()
                                .Text(Text)
                                .FontFamily(FontFamily)
                                .FontSize(32)
                                .SemiBold();
                    });
            });
        }

        #endregion
    }
}
