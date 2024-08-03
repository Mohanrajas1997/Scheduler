using Connector_TSCronJobs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Data;
using System.IO.Compression;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

class Program
{

    public static async Task Main(string[] args)
    {

        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                // Replace "your-api-url" with the actual URL of the API you want to call
                string apiUrl = "http://localhost:5786/Pipeline/GetpplScheduledFinList"; //AppConfig._GetSch_ApiUrl;

                // Make the GET request
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Read and deserialize the response content into a list of anonymous type
                    string content = await response.Content.ReadAsStringAsync();
                    var apiResponseList = JsonConvert.DeserializeObject<List<dynamic>>(content);

                    // Process the list as needed
                    foreach (var item in apiResponseList)
                    {
                        Console.WriteLine($"pipeline_code: {item.pipeline_code}, run_type: {item.run_type}");

                        string srcdb_type = await GetSrcDbtypeAsync(item.pipeline_code);

                        if (srcdb_type == "Excel")
                        {
                            string directoryPath = @"D:\Mohan\Connector\Source_file";

                            // Check if the directory exists
                            if (Directory.Exists(directoryPath))
                            {
                                try
                                {
                                    // Get the files in the directory
                                    string[] files = Directory.GetFiles(directoryPath);

                                    // Loop through each file
                                    foreach (string file in files)
                                    {
                                        // Retrieve file name with extension
                                        string fileNameWithExtension = Path.GetFileName(file);

                                        Console.WriteLine($"File name with extension: {fileNameWithExtension}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"An error occurred: {ex.Message}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Directory '{directoryPath}' does not exist.");
                            }
                        }
                        else
                        {
                            string pipeline_code = item.pipeline_code;
                            // Call the second API for each item in the loop
                           await CallSecondApiAsync1(pipeline_code, "System");
                        }

                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

            }
        }

        static async Task<string> GetSrcDbtypeAsync(string pipelineCode)
        {
            string srcdb_ = "";

            try
            {

                using (HttpClient httpClient = new HttpClient())
                {
                    string ApiUrl = "http://localhost:5786/Pipeline/GetSourcedbType_pplcode?pipeline_code=" + pipelineCode;// AppConfig._TGS_ApiUrl;
                                                                                                                           // Make GET request to the second API
                    HttpResponseMessage response = await httpClient.GetAsync(ApiUrl);
                    

                    // Check if request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read response content as string
                        string responseBody = await response.Content.ReadAsStringAsync();
                        srcdb_ = responseBody;
                        // Do something with the response if needed
                        Console.WriteLine($"Response from GetSrcDbtypeAsync API: {responseBody}");
                    }
                    else
                    {
                        Console.WriteLine($"Error calling GetSrcDbtypeAsync API. Status code: {response.StatusCode}");
                    }
                }
                return srcdb_;
            }
            catch (Exception ex)
            {
                srcdb_ = $"Error calling GetSrcDbtypeAsync API: {ex.Message}";
                return srcdb_;

            }
        }
    }

    static async Task<string> CallExcelSchedulerApiAsync(string pipelineCode, IFormFile file, string initiatedby)
    {
        string msg = "";
        try
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string secondApiUrl = "http://localhost:5786/Pipeline/NewScheduler";// AppConfig._TGS_ApiUrl;

                // Prepare the payload for the second API
                var payload = new { pipeline_code = pipelineCode, file = file, initiated_by = initiatedby };

                // Make the POST request to the second API
                HttpResponseMessage secondApiResponse = await httpClient.PostAsJsonAsync(secondApiUrl, payload);

                if (secondApiResponse.IsSuccessStatusCode)
                {
                    string secondApiContent = await secondApiResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Second API Response: {secondApiContent}");
                }
                else
                {
                    Console.WriteLine($"Error calling second API: {secondApiResponse.StatusCode} - {secondApiResponse.ReasonPhrase}");

                }
            }
            return msg;

        }
        catch (Exception ex)
        {
            msg = ex.Message;
            return msg;
        }
    }

    static async Task CallSecondApiAsync1(string pipelineCode, string initiatedby)
    {
        try
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // Set timeout to 5 minutes (300 seconds)
                httpClient.Timeout = TimeSpan.FromSeconds(300);

                string secondApiUrl = "http://localhost:5786/Pipeline/CreateScheduler";// AppConfig._TGS_ApiUrl;

                // Prepare the payload for the second API
                var payload = new { scheduler_gid = 0, pipeline_code = pipelineCode, initiated_by = initiatedby };

                // Make the POST request to the second API
                HttpResponseMessage secondApiResponse = await httpClient.PostAsJsonAsync(secondApiUrl, payload);

                if (secondApiResponse.IsSuccessStatusCode)
                {
                    string secondApiContent = await secondApiResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Second API Response: {secondApiContent}");
                }

                else
                {
                    Console.WriteLine($"Error calling second API: {secondApiResponse.StatusCode} - {secondApiResponse.ReasonPhrase}");
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling second API: {ex.Message}");
        }
    }



    static void LogError(Exception ex)
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
            writer.WriteLine($"Exception Type: {ex.GetType().FullName}");
            writer.WriteLine($"Message: {ex.Message}");
            writer.WriteLine($"StackTrace: {ex.StackTrace}");
            writer.WriteLine(new string('-', 40)); // Separator between entries
        }

        Console.WriteLine($"Error logged to: {logFilePath}");
    }
    static void LogFile(string sMessage)
    {
        string logFilePath = AppConfig._LogPath; // "D:\\DMS Error Log\\error.log";

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