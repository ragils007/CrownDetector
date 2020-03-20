using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrownDetector.Models
{
    public class HomeViewModel
    {
        public IFormFile AttachedFile { get; set; }
        public string Result { get; set; }
    }
}
