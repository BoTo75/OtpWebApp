using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OtpWebApp
{
    public class File_info
    {
        [Required(ErrorMessage = "fájlnév kötelezõ")]
        public string filename { get; set; }

        [Required(ErrorMessage = "fájl tartalom kötelezõ")]
        public string contentInbase64 { get; set; } 
    }
}
