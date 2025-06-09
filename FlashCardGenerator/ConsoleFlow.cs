
using System.Text;

namespace FlashCardGenerator
{
    internal static class ConsoleFlow
    {

        #region Internal Methods

        // Main Menu: Enter a file for input or exit
        // Done:  Exit or format another file?

        internal static string DoneMenuPrompt(string outputFilePath)
        {
            Console.WriteLine($"Finished generating a PDF at:\n{outputFilePath}\nType 'Exit' to exit or enter any other text to return to Main Menu: ");
            return Console.ReadLine();
        }

        internal static string ErrorPrompt(Exception ex)
        {
            Console.WriteLine($"Encountered an error trying to generate cards from the provided file path.");
            Console.WriteLine($"The application returned the following message:");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine($"\n\nWould you like to try again with another file?\nType 'Exit' to exit or enter any other text to return to Main Menu: \"");

            return Console.ReadLine();
        }

        internal static void ExitMessage()
        {
            Console.WriteLine("Exiting.  Have a nice day!");
            Thread.Sleep(2500);
        }

        internal static string MainMenuPrompt() 
        { 
            StringBuilder sb = new StringBuilder();
            sb.Append($"********** Flash Card Generator Main Menu - {DateTime.Now.ToString("d-MMM-yy HH:mm:ss")} **********\n\n");
            sb.Append($"This application accepts a *.xlsx file with two columns on Sheet1:\n\n");
            sb.Append($"The first cell of column A says FRONT, and each following\ncell contains the text for the front of a card.\n\n");
            sb.Append($"The first cell of column B says BACK, and each following\ncell contains the text for the back of its neighbor from column A.\n\n\n");
            sb.Append($"Enter a Windows file path to read, or type 'Exit' to quit:");
            Console.WriteLine( sb.ToString() );

            return Console.ReadLine();
        }

        internal static string MarginOverridesPrompt(string currentMargins)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"\nThe default page margins are .5\" top, .5\" bottom, .75\" left, and .75\" right.\n");
            sb.Append($"The current settings are: {currentMargins}\n\n");
            sb.Append($"Type \"yes\" to continue, \"defaults\" to return to the original settings,\n");
            sb.Append($"\"tested\" for my personal settings (experimental: trial and error on my own printer),\n");
            sb.Append($"or type a new set of top, bottom, left, and right adjustments separated by commas (e.g., 0.1, 0.2, 0.3, 0.4):");
            Console.WriteLine(sb.ToString());
            return Console.ReadLine();
        }

        internal static string RoundedEdgesPrompt()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"\nDo you want the cards to have rounded edges with thin borders (type \"round\"),\n");
            sb.Append($"or do you want them to have square edges with thick borders that are easier to cut (type \"square\")?,");
            Console.WriteLine(sb.ToString());
            return Console.ReadLine();
        }

        #endregion



    }
}
