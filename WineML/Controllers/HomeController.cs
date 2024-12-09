using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WineML.AI;
using WineML.Models;

namespace WineML.Controllers
{
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

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Predict([FromBody] WineInputModel wineInput)
        {
            try
            {
                var wineSample = new Wine
                {
                    fixed_acidity = wineInput.fixed_acidity,
                    volatile_acidity = wineInput.volatile_acidity,
                    citric_acid = wineInput.citric_acid,
                    residual_sugar = wineInput.residual_sugar,
                    chlorides = wineInput.chlorides,
                    free_sulfur_dioxide = wineInput.free_sulfur_dioxide,
                    total_sulfur_dioxide = wineInput.total_sulfur_dioxide,
                    density = wineInput.density,
                    pH = wineInput.pH,
                    sulphates = wineInput.sulphates,
                    alcohol = wineInput.alcohol,
                    quality = 0, // Value, needed for predicion
                    wine_white = wineInput.color == "white" ? 1 : 0
                };

                var classifier = new Classifier();
                var prediction = classifier.Predict(wineSample);

                return Json(new { quality = prediction });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error predicting wine quality");
                return BadRequest(new { error = ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
