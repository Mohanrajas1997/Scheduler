using System.Data;

namespace Connector_TSCronJobs
{
    public class DataAccess
    {
       
        void LogFile(string sMessage)
        {
            string logFilePath = AppConfig._LogPath;    

            // Ensure the directory exists
            string logDirectory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Append the error information to the log file
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"Timestamp: {DateTime.Now}");

                writer.WriteLine($"Message: {sMessage}");
                writer.WriteLine(new string('-', 40)); // Separator between entries
            }

            Console.WriteLine($"Error logged to: {logFilePath}");
        }
    }
}
