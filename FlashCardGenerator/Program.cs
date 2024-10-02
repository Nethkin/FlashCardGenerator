
using FlashCardGenerator;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

class Program
{
    static void Main(string[] args)
    {
        bool run = true;
        QuestPDF.Settings.License = LicenseType.Community;

        string localOutputDirectory = FileHelper.GetLocalOutputDirectory();

        while (run)
        {
            string userInput = ConsoleFlow.MainMenuPrompt().Replace("\"", "");
            if (userInput.ToLower() == "exit")
            { 
                run = false;
                break;
            }

            string outputFilePath = $@"{localOutputDirectory}\FlashCards_{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf";

            try
            {
                FlashCardData data = FlashCardDataSource.GetCardData(userInput);
                FlashCardTemplate document = new FlashCardTemplate(data);
                document.GeneratePdf(outputFilePath);
            }
            catch (Exception ex)
            {
                if (ConsoleFlow.ErrorPrompt(ex).ToLower() == "exit")
                {
                    run = false;
                    break;
                }
                continue;
            }

            if (ConsoleFlow.DoneMenuPrompt(outputFilePath).ToLower() == "exit")
            {
                run = false;
                break;
            }
        }

        ConsoleFlow.ExitMessage();
    }
}