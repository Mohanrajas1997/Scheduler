using Microsoft.Extensions.Configuration;

namespace Connector_TSCronJobs
{
    public static class AppConfig
    {
        private static IConfiguration _iconfiguration;
        public static string _GetSch_ApiUrl;
        public static string _TGS_ApiUrl;
        public static string _LogPath;

        static AppConfig()
        {
            GetAppSettingsFile();
            _GetSch_ApiUrl = _iconfiguration.GetSection("Settings:GetSch_ApiUrl").Value;
            _TGS_ApiUrl = _iconfiguration.GetSection("Settings:TGS_ApiUrl").Value;
            _LogPath = _iconfiguration.GetSection("Settings:LogPath").Value;
        }

        public static void GetAppSettingsFile()
        {
            //LogFile("step : 1.1.1");
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();

            //return _iconfiguration;
        }
        
        static void LogFile(string sMessage)
        {
            string logFilePath = _LogPath;

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
