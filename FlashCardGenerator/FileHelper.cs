
namespace FlashCardGenerator
{
    internal static class FileHelper
    {
        #region Private Methods

        /// <summary>
        /// Create new folder at specified path. If path not specified,
        /// then create new folder in default Downloads folder named with
        /// current time stamp.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        internal static string GetLocalOutputDirectory(string filePath = "")
        {  
            string localOutputDirectory = string.Empty;
            if (filePath != string.Empty)
            {
                localOutputDirectory = filePath;
            }
            else {
                string defaultDownloadDir = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads\";
                string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                localOutputDirectory = $"{defaultDownloadDir}FlashCards_Output_Dated_{timeStamp}";
            }
            if (!Directory.Exists(localOutputDirectory))
            {
                Directory.CreateDirectory(localOutputDirectory);
            }

            return localOutputDirectory;
        }

        #endregion
    }
}
