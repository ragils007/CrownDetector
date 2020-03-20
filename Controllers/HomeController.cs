using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CrownDetector.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;

namespace CrownDetector.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MakePredictionRequest(HomeViewModel model)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid Prediction-Key.
            client.DefaultRequestHeaders.Add("Prediction-Key", "de8bf3a1c0e34c6ab6c1cb3b2a43a423");

            // Prediction URL - replace this example URL with your valid Prediction URL.
            string url = "https://crowndetector002.cognitiveservices.azure.com/customvision/v3.0/Prediction/9bf7ca7f-3df0-4a49-9088-98e7527236f0/classify/iterations/Iteration1/image";

            HttpResponseMessage response;

            byte[] byteData;

            using (var ms = new MemoryStream())
            {
                model.AttachedFile.CopyTo(ms);
                byteData = ms.ToArray();
                //string s = Convert.ToBase64String(fileBytes);
                // act on the Base64 data
            }

            // Request body. Try this sample with a locally stored image.
            

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                model.Result = await response.Content.ReadAsStringAsync();
            }

            return View("Index", model);
        }
    }
}
