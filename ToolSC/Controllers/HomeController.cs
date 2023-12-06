using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToolSC.Helpers;
using ToolSC.Models;
using ToolSC.Requests;

namespace ToolSC.Controllers
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] ColumnRequest request)
        {
            // validate
            if (string.IsNullOrEmpty(request.Input))
            {
                return Json(new ResponseModel<List<TableColumn>> { Status = 0, Msg = "Please enter the input" });
            }

            var data = CommonHelpers.GetNameAndTypeOfColumn(request.Input);

            try
            {
                return Json(new ResponseModel<List<TableColumn>> { Status = 1, Data = data });
            }
            catch (Exception)
            {
                return Json(new ResponseModel<List<TableColumn>> { Status = 0 });
            }
        }
    }
}