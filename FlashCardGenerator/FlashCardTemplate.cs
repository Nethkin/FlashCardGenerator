using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data.Common;
using System.Reflection;

namespace FlashCardGenerator
{
    public class FlashCardTemplate : IDocument
    {
        #region Private Members

        private float _cardWidth = 3.5f;    // Inches
        private float _cardHeight = 2.5f;   // Inches
        private string _fontFamily;
        private float _margin = .5f;        // Inches
        private int _numberOfColumns;

        #endregion

        #region Public Properties

        public FlashCardData Data { get; set; }

        #endregion

        #region Constructors

        public FlashCardTemplate(FlashCardData data, int numberOfColumns = 2, string fontFamily = "SBL Hebrew")
        {
            Data = data;
            _fontFamily = fontFamily;
            _numberOfColumns = numberOfColumns;
        }

        #endregion

        #region Private Methods

        void ComposeContent(IContainer container)
        {
            container
                .AlignCenter()
                .AlignMiddle()
                .Column(column =>
                {
                    column.Item().Element(ComposeTable);
                });
        }

        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                // Step 1 --> define number and sizes of columns.
                table.ColumnsDefinition(columns =>
                {
                    for (int i = 0; i < _numberOfColumns; i++)
                    {
                        columns.ConstantColumn(_cardWidth, Unit.Inch);
                    }
                });

                // Step 2 --> implement table's header, which repeats on each page.
                // No Header in this template.

                // Step 3 --> use a loop to iterate over data source and add cells for each.
                for (int i = 0; i < Data.Terms.Count; i ++)
                {
                    table.Cell()
                    .Row(row =>
                    {
                        row.RelativeItem()
                            .Height(_cardHeight, Unit.Inch)
                            .Component(new CardComponent(Data.Terms[i], _fontFamily));
                    });
                }
            });
        }

        #endregion

        #region Public Methods

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(_margin, Unit.Inch);
                    page.Content().Element(ComposeContent);
                });
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        #endregion
    }
}
