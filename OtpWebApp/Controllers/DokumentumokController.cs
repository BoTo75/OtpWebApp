using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using OtpWebApp.Services;

namespace OtpWebApp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DokumentumokController : ControllerBase
    {
        private readonly IFileServices _fileService;
        private readonly ILogger<DokumentumokController> _logger;
        
        public DokumentumokController(ILogger<DokumentumokController> logger, IFileServices fileService)
        {
            _fileService = fileService;               
            _logger = logger;            
        }

        // GET api/dokumentumok
        [HttpGet]
        public ActionResult<Dictionary<string, string>> Get()
        {
            _logger.LogInformation("Az összes file letöltve");
            return _fileService.CollectFiles("*.*");
        }

        // GET api/dokumentumok/filename
        [HttpGet("{filename}")]
        public ActionResult<Dictionary<string, string>> Get(string filename)
        {
            _logger.LogInformation(String.Format("A {0} file letöltve", filename));
            return _fileService.CollectFiles(filename);
        }

        [HttpPost]
        public ActionResult<Dictionary<string, string>> Post([FromBody] File_info[] files)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            foreach (var file in files)
            {
                string info = _fileService.SaveFile(file);
                 response.Add(file.filename, info);

                if("ilyen néven már létezik a fájl".Equals(info))
                    _logger.LogInformation(String.Format("{0} --> {1}", file.filename, info));
                else
                    _logger.LogInformation(String.Format("A {0} file feltöltésre került", file.filename));
            }
            return response;
        }

        
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

       
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
