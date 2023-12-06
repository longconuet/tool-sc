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
        public async Task<IActionResult> GetColumnDetail([FromBody] ColumnRequest request)
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

        [HttpPost]
        public async Task<IActionResult> GenTableData([FromBody] ColumnRequest request)
        {
            // validate
            if (string.IsNullOrEmpty(request.Input))
            {
                return Json(new ResponseModel<List<string>> { Status = 0, Msg = "Please enter the input" });
            }

            List<string> existList = new();

            var columns = CommonHelpers.GetNameAndTypeOfColumn(request.Input);
            if (columns.Any())
            {
                foreach (var column in columns)
                {
                    string columnData;
                    string columnType = column.Type;
                    if (columnType == "int" || columnType == "bigint")
                    {
                        columnData = CommonHelpers.GenerateColumnDataNumber(column, existList);
                    }
                    else
                    {
                        if (int.Parse(column.Length) <= 10)
                        {
                            columnData = CommonHelpers.GenerateColumnDataFromRandomString(column, existList);
                        }
                        else
                        {
                            columnData = CommonHelpers.GenerateColumnDataFromColumnName(column, existList);
                        }
                    }

                    existList.Add(columnData);
                }
            }

            try
            {
                return Json(new ResponseModel<List<string>> { Status = 1, Data = existList });
            }
            catch (Exception)
            {
                return Json(new ResponseModel<List<string>> { Status = 0 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenTableDataFullLength([FromBody] ColumnRequest request)
        {
            // validate
            if (string.IsNullOrEmpty(request.Input))
            {
                return Json(new ResponseModel<List<string>> { Status = 0, Msg = "Please enter the input" });
            }

            List<string> existList = new();

            var columns = CommonHelpers.GetNameAndTypeOfColumn(request.Input);
            if (columns.Any())
            {
                foreach (var column in columns)
                {
                    string columnData;
                    string columnType = column.Type;
                    if (columnType == "int" || columnType == "bigint")
                    {
                        columnData = CommonHelpers.GenerateColumnDataNumber(column, existList);
                    }
                    else
                    {
                        columnData = CommonHelpers.GenerateColumnDataFullLengthFromColumnName(column, existList);
                    }

                    existList.Add(columnData);
                }
            }

            try
            {
                return Json(new ResponseModel<List<string>> { Status = 1, Data = existList });
            }
            catch (Exception)
            {
                return Json(new ResponseModel<List<string>> { Status = 0 });
            }
        }
    }
}