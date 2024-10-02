
using ClosedXML.Excel;
using System.Data;
using System.Text;

namespace FlashCardGenerator
{
    public static class FlashCardDataSource
    {
        #region Private Methods

        private static IDictionary<string, IList<string>> DataTableToDictionary(DataTable dataTable)
        {
            IDictionary<string, IList<string>> output = new Dictionary<string, IList<string>>();
            foreach (DataColumn column in dataTable.Columns)
            {
                string header = column.ColumnName;
                output.Add(header, new List<string>());

                foreach (DataRow row in dataTable.Rows)
                {
                    output[header].Add((string)row[header]);
                }
            }

            return output;
        }

        private static DataTable ReadWorksheetWithClosedXML
            (
                string filePath,
                string worksheetName = "Sheet1"
            )
        {
            DataTable dataTable = new DataTable();
            XLWorkbook workbook;
            IXLWorksheet worksheet;

            // Instantiate the XLWorkbook object from the file path
            workbook = new XLWorkbook(filePath);

            // Get the specified worksheet
            worksheet = workbook.Worksheet(worksheetName);

            IXLRow firstRowUsed = worksheet.FirstRowUsed();
            IXLRangeRow headerRow = firstRowUsed.RowUsed();
            IXLRangeRow dataRow = headerRow.RowBelow();

            int numberOfColumns = headerRow.CellCount();

            // Create the header of data table
            //Console.WriteLine("Found headers: ");
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 1; i <= numberOfColumns; i++)
            {
                string headerText = headerRow.Cell(i).GetString();
                dataTable.Columns.Add(headerText, typeof(string));

                stringBuilder.Append($"{headerText}|");
            }

            //Console.WriteLine(stringBuilder.ToString());
            stringBuilder.Clear();

            // Loop through each row and add cell values to the data table
            //Console.WriteLine("Data Cells:");
            while (!dataRow.Cell(1).IsEmpty())
            {
                DataRow newDataTableRow = dataTable.NewRow();
                for (int i = 1; i <= numberOfColumns; i++)
                {
                    string cellValue = dataRow.Cell(i).GetFormattedString();
                    newDataTableRow[i - 1] = cellValue;

                    stringBuilder.Append($"{newDataTableRow[i - 1]}|");
                }
                dataTable.Rows.InsertAt(newDataTableRow, dataTable.Rows.Count + 1);

                //Console.WriteLine(stringBuilder.ToString());
                stringBuilder.Clear();

                dataRow = dataRow.RowBelow();
            }

            // Now close the workbook and return the data table
            workbook.Dispose();
            return dataTable;
        }

        private static IList<string> ReorderColumnsToList(IList<string> front, IList<string> back, int columnsPerPage = 2, int rowsPerPage = 4) 
        {
            int cardsPerPage = columnsPerPage * rowsPerPage;
            string blank = string.Empty;
            
            // Make sure each list count is equally divided by the number of cards per page.
            while (front.Count % cardsPerPage != 0)
            {
                front.Add(blank);
            }
            while (back.Count % cardsPerPage != 0)
            {
                back.Add(blank);
            }

            // Make sure the front and back lists have equal counts.
            if (front.Count != back.Count)
            {
                while (front.Count < back.Count)
                {
                    front.Add(blank);
                }
                while (back.Count < front.Count)
                {
                    back.Add(blank);
                }
            }

            IList<string> combined = new List<string>(new string[front.Count + back.Count]);

            for (int oldIndex = 0; oldIndex < front.Count; oldIndex++)
            {
                int remainder;
                int intQuotient = Math.DivRem(oldIndex, cardsPerPage, out remainder);
                int newIndex = oldIndex + intQuotient * cardsPerPage;
                combined[newIndex] = front[oldIndex];
            }

            for(int startIndex = 0; startIndex < back.Count; startIndex+= columnsPerPage)
            {
                int subIndex = 0;
                for (int columnIndex = columnsPerPage - 1; columnIndex >= 0; columnIndex--)
                {
                    int oldIndex = subIndex + startIndex;
                    int remainder;
                    int intQuotient = Math.DivRem(oldIndex, cardsPerPage, out remainder);
                    // This little algorithm was a PITA to solve; trust me.
                    int newIndex = startIndex + intQuotient * cardsPerPage + columnIndex + cardsPerPage;
                    combined[newIndex] = back[oldIndex];
                    subIndex++;
                }
            }

            return new List<string>(combined);
        }

        private static IList<string> GenerateList(int numberOfCards, string face)
        {
            IList<string> cardData = new List<string>();
            for (int i = 1; i <= numberOfCards; i++)
            {
                cardData.Add($"{face} #{i}");
            }
            return cardData;
        }

        #endregion

        public static FlashCardData GetCardData(string filePath, int columnsPerPage = 2, int rowsPerPage = 4)
        {
            const string front = "FRONT";
            const string back = "BACK";
            IDictionary<string, IList<string>> dictionaryFromFile = DataTableToDictionary(ReadWorksheetWithClosedXML(filePath));

            IList<string> reordered = ReorderColumnsToList(dictionaryFromFile[front], dictionaryFromFile[back], columnsPerPage, rowsPerPage);

            return new FlashCardData
            {
                Terms = new List<string>(reordered)
            };
        }
    }
}
