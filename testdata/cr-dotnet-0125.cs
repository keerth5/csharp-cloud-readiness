using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudProtocolExample
{
    // ❌ Violating Example: Using legacy FTP protocol
    public class LegacyFtpClient
    {
        public void UploadFile()
        {
            // Violation: FTP protocol is not supported or reliable in many cloud environments
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://example.com/file.txt");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential("user", "password");

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.WriteLine("This is a test file.");
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"[Violation] Upload status: {response.StatusDescription}");
            }
        }
    }

    // ❌ Violating Example: Using SMB/UNC path (\\server\share)
    public class NetworkFileAccess
    {
        public void ReadFileFromShare()
        {
            string uncPath = @"\\corpserver\shared\data.txt";

            // Violation: SMB/UNC paths rely on on-prem network protocols
            if (File.Exists(uncPath))
            {
                string content = File.ReadAllText(uncPath);
                Console.WriteLine($"[Violation] Read from UNC path: {uncPath}");
                Console.WriteLine(content);
            }
            else
            {
                Console.WriteLine("[Violation] UNC share not accessible in cloud environment.");
            }
        }
    }

    // ✅ Compliant Example: Using HTTPS for file transfer
    public class SecureHttpClient
    {
        private readonly HttpClient httpClient = new HttpClient();

        public async Task UploadFileSecurelyAsync()
        {
            Console.WriteLine("[Compliant] Uploading file via HTTPS to cloud API...");

            byte[] fileData = System.Text.Encoding.UTF8.GetBytes("This is a secure file upload.");
            HttpContent content = new ByteArrayContent(fileData);

            // ✅ Use HTTPS endpoint for file operations
            HttpResponseMessage response = await httpClient.PostAsync("https://storage.example.com/upload", content);
            Console.WriteLine($"[Compliant] Secure upload response: {response.StatusCode}");
        }
    }

    // ✅ Compliant Example: Using cloud-native storage SDKs (Azure/AWS/GCP)
    public class CloudStorageExample
    {
        public async Task UploadToCloudAsync()
        {
            Console.WriteLine("[Compliant] Using cloud SDK for file storage...");

            // Example placeholder (e.g., Azure Blob, AWS S3, GCP Storage)
            // This ensures compatibility with cloud networking and identity management.
            await Task.Delay(500); // Simulate upload
            Console.WriteLine("[Compliant] File uploaded to cloud storage using SDK.");
        }
    }
}
