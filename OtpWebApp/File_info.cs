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
        [Required(ErrorMessage = "f�jln�v k�telez�")]
        public string filename { get; set; }

        [Required(ErrorMessage = "f�jl tartalom k�telez�")]
        public string contentInbase64 { get; set; } 
    }
}
