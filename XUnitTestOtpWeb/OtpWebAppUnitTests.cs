using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using Xunit;
using OtpWebApp;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace XUnitTestOtpWeb
{
    public class OtpWebAppUnitTests
    {

        

        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly string url = "api/dokumentumok";
        private readonly string _filename = "Teszt.txt";
        private readonly string _fileContent = "dGVzenQ=";
        private Microsoft.Extensions.Configuration.IConfiguration _config;


        public OtpWebAppUnitTests()
        {
            _server = new TestServer(new WebHostBuilder()
               .UseStartup<Startup>());
            _client = _server.CreateClient();
            deleteTestFile();
        }

        private void deleteTestFile()
        {
            
                var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();
            string fullFilename = Path.Combine(
                configuration.GetSection("AppSettings").GetSection("fileFolder").Value, _filename);
            if (File.Exists(fullFilename))
                File.Delete(fullFilename);
        }

        [Fact]
        public async Task TestGetAllDocument()
        {
            
            var response = await _client.GetAsync(url);

            
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task TestGetNonExistentFile()
        {
           
            var response = await _client.GetAsync(url + "/unit_teszt.txt");                    
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>
                (await response.Content.ReadAsStringAsync());
            
            Assert.Equal("nem létezõ állomány", values["*info"]);
        }

        [Fact]
        public async Task TestGetAnexistingFile()
        {
            
            var response = await _client.GetAsync(url);
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>
                (await response.Content.ReadAsStringAsync());
            string filename = values.FirstOrDefault().Key;
            response = await _client.GetAsync(url + "/" + filename);           
            
             values = JsonConvert.DeserializeObject<Dictionary<string, string>>
                (await response.Content.ReadAsStringAsync());
            
            Assert.Equal(filename, values.First().Key);
        }

        [Fact]
        public void TestPostMissingFileContent()
        {
            
            File_info[] fileinfo = new File_info[1];
            fileinfo[0] = new File_info()
            {
                filename = _filename,
                contentInbase64 = string.Empty
            };

            var status = PostFile(fileinfo);

            
            Assert.Equal("fájl tartalom kötelezõ", status["hiba"]);
        }

        [Fact]
        public void TestPostMissingFilename()
        {
            
            File_info[] fileinfo = new File_info[1];
            fileinfo[0] = new File_info()
            {
                filename = string.Empty,
                contentInbase64 = _fileContent
            };

            var status = PostFile(fileinfo);

           
            Assert.Equal("fájlnév kötelezõ", status["hiba"]);
        }

        [Fact]
        public void TestPostFile()
        {
            File_info[] fileinfo = new File_info[1];
            fileinfo[0] = new File_info()
            {
                filename = _filename,
                contentInbase64 = _fileContent
            };

            var status = PostFile(fileinfo);

           
            Assert.Equal("letárolva", status[_filename]);
        }


        public Dictionary<string, string> PostFile(File_info[] requestObj)
        {             
            Dictionary<string, string> responseObj = new Dictionary<string, string>();             
            HttpResponseMessage response = new HttpResponseMessage();
            response = _client.PostAsJsonAsync("api/dokumentumok", requestObj).Result;
            if (response.IsSuccessStatusCode)
            {
                
                string result = response.Content.ReadAsStringAsync().Result;
                responseObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            }

            else
            {
                JToken token = JObject.Parse(response.Content.ReadAsStringAsync().Result);                
                responseObj.Add("hiba", token.SelectToken("errors").First.First.First.ToString());
            }


            return responseObj;
        }       
    }

    public class File_info
    {
        public string filename { get; set; }

        public string contentInbase64 { get; set; }
    }
}
