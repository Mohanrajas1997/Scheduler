using Connector_Taskscheduler;
using Newtonsoft.Json;
using System.Data;
using System.IO.Compression;
using System.Net.Http.Json;
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
                string apiUrl = "http://localhost:5786/Pipeline/GetSchedulerList"; //AppConfig._GetSch_ApiUrl;

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
                        Console.WriteLine($"scheduler_gid: {item.scheduler_gid}, scheduled_date: {item.scheduled_date}, pipeline_code: {item.pipeline_code}, scheduler_status: {item.scheduler_status}");

                        // Call the second API for each item in the loop
                        await CallSecondApiAsync(item.scheduler_gid, item.pipeline_code, item.scheduler_initiated_by);
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
                Console.ReadLine();

            }
        }

        static async Task CallSecondApiAsync(int schedulerGid, string pipelineCode, string initiatedby)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // Set timeout to 5 minutes (300 seconds)
                    httpClient.Timeout = TimeSpan.FromSeconds(300);

                    string secondApiUrl = "http://localhost:5786/Pipeline/TGDS_Taskscheduler";// AppConfig._TGS_ApiUrl;

                    // Prepare the payload for the second API
                    var payload = new { scheduler_gid = schedulerGid, pipeline_code = pipelineCode, initiated_by = initiatedby };

                    // Make the POST request to the second API
                    HttpResponseMessage secondApiResponse = await httpClient.PostAsJsonAsync(secondApiUrl, payload);

                    string apiUrlForResch = "http://localhost:5786/Pipeline/Reschedulefornexttime?pipelinecode=" + Uri.EscapeDataString(pipelineCode);

                    // Make the GET request and await the response
                    HttpResponseMessage reschedApiResponse = await httpClient.GetAsync(apiUrlForResch);

                    // Check if the response is successful
                    if (reschedApiResponse.IsSuccessStatusCode)
                    {
                        // Read the content of the response
                        string reschedApiContent = await reschedApiResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"Reschedule API Response: {reschedApiContent}");
                    }
                    else
                    {
                        // Handle unsuccessful response (e.g., log error)
                        Console.WriteLine($"Error: {reschedApiResponse.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling second API: {ex.Message}");
                Console.ReadLine();
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

