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
        private bool Rounded { get; }
        #endregion

        #region Constructors

        public CardComponent(string text, string fontFamily, bool rounded = false)
        {
            FontFamily = fontFamily;
            Text = text;
            Rounded = rounded;
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
                        float strokeWidth;
                        float cornerRadius;
                        if (Rounded)
                        {
                            strokeWidth = 2;
                            cornerRadius = 20;
                        }
                        else
                        {
                            strokeWidth = 10;
                            cornerRadius = 0;
                        }
                        using var paint = new SKPaint
                        {
                            Color = SKColor.Parse(color),
                            IsStroke = isStroke,
                            StrokeWidth = strokeWidth,
                            IsAntialias = true
                        };
                        canvas.DrawRoundRect(0, 0, size.Width, size.Height, cornerRadius, cornerRadius, paint);
                    }
                });

                layers
                    .PrimaryLayer()
                    .AlignCenter()
                    .AlignMiddle()
                    .Padding(.25f, Unit.Inch)
                    .Column(column =>
                    {
                        column.Item()
                                .ScaleToFit()
                                .Text(Text)
                                .FontFamily(FontFamily)
                                .FontSize(28)
                                .SemiBold();
                    });
            });
        }

        #endregion
    }
}
