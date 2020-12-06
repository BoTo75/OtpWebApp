using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OtpWebApp.Services
{
    public interface IFileServices
    {        
        string SaveFile(File_info fileModel);
        string Base64Encode(string FullName);
        Dictionary<string, string> CollectFiles(string searchPattern);
    }

    public class FileServices : IFileServices
    {
        private string _path;
        private readonly ILogger<FileServices> _logger;
        
        public FileServices(ILogger<FileServices> logger, IConfiguration configuration) 
        {
            if (string.IsNullOrEmpty(configuration.GetSection("AppSettings").GetSection("fileFolder").Value))
            {
                configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();               
            }
           
            _path = configuration.GetSection("AppSettings").GetSection("fileFolder").Value;
            
            _logger = logger;
        }
        public Dictionary<string, string> CollectFiles(string searchPattern)
        {
            Dictionary<string, string> fileList = new Dictionary<string, string>();

            DirectoryInfo directory = new DirectoryInfo(_path);
            FileInfo[] Files = directory.GetFiles(searchPattern);
            foreach (FileInfo file in Files)
            {
                fileList.Add(file.Name, Base64Encode(file.FullName));
            }
            if (fileList.Count == 0)
                fileList.Add("*info", "nem létező állomány");
            return fileList;
        }

        public string SaveFile(File_info fileinfo)
        {
            string fullFilename = Path.Combine(_path, fileinfo.filename);
            if (!File.Exists(fullFilename))
            {
                try
                {
                    File.WriteAllBytes(fullFilename, Convert.FromBase64String(fileinfo.contentInbase64));
                    return "letárolva";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return "szerver oldali hiba";
                }
            }
            else
                return "ilyen néven már létezik a fájl";
        }

        public string Base64Encode(string FullName)
        {
            string fileInBase64 = string.Empty;
            try
            {
                fileInBase64 = Convert.ToBase64String(File.ReadAllBytes(FullName));
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return fileInBase64;
        }
    }
}
