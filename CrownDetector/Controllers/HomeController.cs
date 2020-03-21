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
using System.Drawing;
using CrownDetector.DB;
using Msdfa.Data.Raw;

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
            if (model.AttachedFile == null)
                return View("Index", model);

            

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", _config.Value.Key);

            string url = _config.Value.Url;

            HttpResponseMessage response;

            var combinedPath = Path.Combine(_hostingEnvironment.WebRootPath, "temp");
            var fileName = "org" + DateTime.Now.ToString("yyyyMMddhhmmss") + model.AttachedFile.FileName;
            var fileNameNew = DateTime.Now.ToString("yyyyMMddhhmmss") + model.AttachedFile.FileName;
            var filePath = Path.Combine(combinedPath, fileName);
            var filePathNew = Path.Combine(combinedPath, fileNameNew);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.AttachedFile.CopyTo(stream);
            }

            byte[] byteData = GetImageAsByteArray(filePath);

            JsonObj result = null;

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                var res = await response.Content.ReadAsStringAsync();

                result = JsonConvert.DeserializeObject<JsonObj>(res);
            }
            
            var probability = result.Predictions.Where(x => x.TagName == "Pneumonia_Covid19").Select(x => x.Probability).FirstOrDefault();

            var czyWykryto = false;

            if (probability > _config.Value.Probability)
            {
                await SecondScan(filePath, filePathNew);
                czyWykryto = true;
                model.Result = $"<p class=\"text-danger info\">Additional patient health verification required </p>";
                model.Src = $"/temp/{fileNameNew}";
                model.Class = "zdjecie";
            }
            else
            {
                model.Src = $"/temp/{fileName}";
                model.Class = "";
            }

            using (var cc = CC.GetCorona())
            {
                var img = System.IO.File.ReadAllBytes(filePath);
                byte[] img2 = null;
                if(czyWykryto)
                    img2 = System.IO.File.ReadAllBytes(filePathNew);

                var newItem = new corona
                {
                    imie = model.Imie,
                    nazwisko = model.Nazwisko,
                    pesel = model.Pesel,
                    opis = model.Opis,
                    data_po = model.Data_po,
                    foto = img,
                    foto_marked = img2,
                };
                newItem.Save(cc.Cnn);
            }

            model.Procent = Math.Round(((decimal)probability * 100m) / 1m, 2);

            return View("Index", model);
        }

        private async Task SecondScan(string filePath, string filePathNew)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", _config.Value.Key);

            string url = _config.Value.Url2;

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(filePath);

            JsonObj2 result = null;

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                var res = await response.Content.ReadAsStringAsync();

                result = JsonConvert.DeserializeObject<JsonObj2>(res);
            }

            Bitmap originalBmp = (Bitmap)Image.FromFile(filePath);

            Bitmap tempBitmap = new Bitmap(originalBmp, originalBmp.Width, originalBmp.Height);

            using (Graphics g = Graphics.FromImage(tempBitmap))
            {
                var photoW = originalBmp.Width;
                var photoH = originalBmp.Height;
                Pen redPen = new Pen(Color.Red, 3);
                var points = result.predictions.ToList();
                foreach (var point in points.Where(x => x.probability > _config.Value.Probability2).ToList())
                {
                    int x = Convert.ToInt32(photoW*point.boundingBox.left);
                    int y = Convert.ToInt32(photoH*point.boundingBox.top);
                    int w = Convert.ToInt32(photoW*point.boundingBox.width);
                    int h = Convert.ToInt32(photoH*point.boundingBox.height);
                    Rectangle rect = new Rectangle(x, y, w, y);
                    g.DrawRectangle(redPen, rect);
                }
            }

            Image image = (Image)tempBitmap;
            image.Save(filePathNew);
        }

        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        public IActionResult Lista()
        {
            var model = new ListViewModel();

            using (var cc = CC.GetCorona())
            {
                var dataList = cc.Cnn.Query<corona>("SELECT * FROM corona").ToList();

                model.ListItems = dataList.Select(x => new ListItem()
                {
                    Imie = x.imie,
                    Nazwisko = x.nazwisko,
                    Opis = x.opis,
                    Pesel = x.pesel,
                    Data_po = x.data_po
                }).ToList();
            }

            return View(model);
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

    public class JsonObj2
    {
        public string id { get; set; }
        public string project { get; set; }
        public string iteration { get; set; }
        public string created { get; set; }
        public List<JsonPoints> predictions { get; set; }
    }

    public class JsonPoints
    {
        public float probability { get; set; }
        public string tagId { get; set; }
        public string tagName { get; set; }
        public JsonPoint boundingBox { get; set; }
    }

    public class JsonPoint
    {
        public float left { get; set; }
        public float top { get; set; }
        public float width { get; set; }
        public float height { get; set; }
    }
}
