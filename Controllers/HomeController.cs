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
using Microsoft.Extensions.Options;
using CrownDetector.Options;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace CrownDetector.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptions<PredictionAPI> _config;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(ILogger<HomeController> logger, IOptions<PredictionAPI> config, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _config = config;
            _hostingEnvironment = hostingEnvironment;
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
            client.DefaultRequestHeaders.Add("Prediction-Key", _config.Value.Key);

            // Prediction URL - replace this example URL with your valid Prediction URL.
            string url = _config.Value.Url;

            HttpResponseMessage response;

            var combinedPath = Path.Combine(_hostingEnvironment.WebRootPath, "temp");
            var fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + model.AttachedFile.FileName;
            var filePath = Path.Combine(combinedPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.AttachedFile.CopyTo(stream);
            }

            byte[] byteData = GetImageAsByteArray(filePath);

            //using (var ms = new MemoryStream())
            //{
            //    model.AttachedFile.CopyTo(ms);
            //    byteData = ms.ToArray();
            //    //string s = Convert.ToBase64String(fileBytes);
            //    // act on the Base64 data
            //}

            // Request body. Try this sample with a locally stored image.

            JsonObj result = null;

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                var res = await response.Content.ReadAsStringAsync();

                result = JsonConvert.DeserializeObject<JsonObj>(res);
            }

            var probability = result.Predictions.Where(x => x.TagName == "Pneumonia_Covid19").Select(x => x.Probability).FirstOrDefault();

            if(probability > _config.Value.Probability)
                model.Result = $"<p class=\"text-danger\">Uwaga konieczna dodatkowa weryfikacja zdrowia pacjenta</p><img src=\"/temp/{fileName}\" class=\"zdjecie\"/>";
            else
                model.Result = $"<img src=\"/temp/{fileName}\"/>";

            return View("Index", model);
        }
        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

    }

    public class JsonObj
    {
        public string Id { get; set; }
        public string Project { get; set; }
        public string Iteration { get; set; }
        public string Created { get; set; }
        public List<JsonObjItem> Predictions { get; set; }
    }

    public class JsonObjItem
    {
        public string TagId { get; set; }
        public string TagName { get; set; }
        public float Probability { get; set; }
    }
}
