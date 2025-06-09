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

        private float _topMargin;
        private float _bottomMargin;
        private float _leftMargin;
        private float _rightMargin;
        
        private float _cardWidth = 3.5f;    // Inches
        private float _cardHeight = 2.5f;   // Inches
        private string _fontFamily;
        private int _numberOfColumns;

        // Default margins (inches).
        private float _defaultBottom = .5f;
        private float _defaultLeft = .75f;
        private float _defaultRight = .75f;
        private float _defaultTop = .5f;

        // Default margin overrides (inches) based on trial and error with my printer.
        private float _bottomOverride =.46175f;
        private float _leftOverride = .6965f;
        private float _rightOverride = .8035f;
        private float _topOverride = .53825f;

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

            float topMargin = _defaultTop;
            float bottomMargin = _defaultBottom;
            float leftMargin = _defaultLeft;
            float rightMargin = _defaultRight;

            bool satisfied = false;
            string userInput;
            do
            {
                string currentMargins = $"{topMargin}\" top, {bottomMargin}\" bottom, {leftMargin}\" left, {rightMargin}\" right";
                userInput = ConsoleFlow.MarginOverridesPrompt(currentMargins).Replace("\"", "").ToLower();

                switch (userInput)
                {
                    case "yes":
                        satisfied = true;
                        break;
                    case "defaults":
                        topMargin = _defaultTop;
                        bottomMargin = _defaultBottom;
                        leftMargin = _defaultLeft;
                        rightMargin = _defaultRight;
                        break;
                    case "tested":
                        topMargin = _topOverride;
                        bottomMargin = _bottomOverride;
                        leftMargin = _leftOverride;
                        rightMargin = _rightOverride;
                        break;
                    default:
                        // Split the input and trim spaces
                        string[] parts = userInput.Split(',');
                        float[] numbers = new float[4];

                        if (parts.Length != 4)
                        {
                            Console.WriteLine($"Error: Please enter \"yes\", \"reset\", or exactly four numbers separated by commas.");
                            break;
                        }

                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (!float.TryParse(parts[i].Trim(), out numbers[i]))
                            {
                                Console.WriteLine($"Error: \"{parts[i].Trim()}\" is not a valid number.");
                                break;
                            }
                        }
                        topMargin = numbers[0];
                        bottomMargin = numbers[1];
                        leftMargin = numbers[2];
                        rightMargin = numbers[3];
                        break;
                }
            } while (!satisfied);

            _bottomMargin = bottomMargin;
            _leftMargin = leftMargin;
            _rightMargin = rightMargin;
            _topMargin = topMargin;
            Console.WriteLine($"\nContinuing with the following margin settings:\nLeft: {_leftMargin}\"\nRight: {_rightMargin}\"\nTop: {_topMargin}\"\nBottom: {_bottomMargin}\"");
        }

        #endregion

        #region Private Methods

        void ComposeContent(IContainer container)
        {
            container
                .Column(column =>
                {
                    column.Item().Element(ComposeTable);
                });
        }

        void ComposeTable(IContainer container)
        {
            bool roundedEdges = false;
            bool satisfied = false;
            do
            {
                string userInput = ConsoleFlow.RoundedEdgesPrompt().Replace("\"", "").ToLower();
                switch (userInput)
                {
                    case "round":
                        Console.WriteLine("Continuing with rounded edges.");
                        roundedEdges = true;
                        satisfied = true;
                        break;
                    case "square":
                        Console.WriteLine("Continuing with square edges.");
                        roundedEdges = false;
                        satisfied = true;
                        break;
                    default:
                        Console.WriteLine($"Error: Please type \"round\" or \"square\".");
                        break;
                }
            } while (!satisfied);
            

            container.Table(table =>
            {
                // Step 1 --> define number and sizes of columns.
                table.ColumnsDefinition(columns =>
                {
                    for (int i = 0; i < _numberOfColumns; i++)
                    {
                        // I think this fudge factor accounts for two 2-point border lines,
                        // but in the recent update to CardComponent, it could be
                        // either 2 or 10 points.  It should be negligible.
                        columns.ConstantColumn(_cardWidth + 2 * 2 / 72, Unit.Inch);
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
                            .Component(new CardComponent(Data.Terms[i], _fontFamily, roundedEdges));
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
                    page.MarginLeft(_leftMargin, Unit.Inch);
                    page.MarginRight(_rightMargin, Unit.Inch);
                    page.MarginTop(_topMargin, Unit.Inch);
                    page.MarginBottom(_bottomMargin, Unit.Inch);
                    page.Content().Element(ComposeContent);
                });
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        #endregion
    }
}
