using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Diagnostics;
using ToolSC.Helpers;
using ToolSC.Models;
using ToolSC.Requests;

namespace ToolSC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly List<string> _sysVars;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _sysVars = CommonHelpers.GetSystemVariables();
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

            return Json(new ResponseModel<List<TableColumn>> { Status = 1, Data = data });
        }

        [HttpPost]
        public async Task<IActionResult> GenTableData([FromBody] ColumnRequest request)
        {
            // validate
            if (string.IsNullOrEmpty(request.Input))
            {
                return Json(new ResponseModel<TableDataModel> { Status = 0, Msg = "Please enter the input" });
            }

            TableDataModel data = new();
            List<string> existList = new();
            string columnKeyData = "";

            var columns = CommonHelpers.GetNameAndTypeOfColumn(request.Input);
            if (columns.Any())
            {
                foreach (var column in columns)
                {
                    if (column.Name == Const.SITE_CODE_KEY)
                    {
                        existList.Add(!string.IsNullOrEmpty(request.SiteCode) ? request.SiteCode : Const.DEFAULT_SITE_CODE);
                        continue;
                    }

                    if (_sysVars.Contains(column.Name))
                    {
                        continue;
                    }

                    var manualColumn = request.ManualData.FirstOrDefault(x => x.Name == column.Name);
                    if (manualColumn != null && !string.IsNullOrEmpty(manualColumn.Data))
                    {
                        existList.Add(manualColumn.Data);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(request.ColumnKey) && column.Name == request.ColumnKey)
                    {
                        columnKeyData = CommonHelpers.GenColumnKeyData(int.Parse(column.Length), 1);
                        existList.Add(columnKeyData);
                        continue;
                    }

                    string columnData;
                    string columnType = column.Type;
                    if (columnType == "int" || columnType == "bigint" || columnType == "float" || columnType == "decimal")
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

            existList.AddRange(CommonHelpers.GenSystemData(request.KinoId, request.SystemName));
            data.DataList = existList;
            data.Data = CommonHelpers.CombineDataString(existList);
            data.DataColumn = CommonHelpers.ConvertDataToColumn(existList);

            // multi record
            if (request.NumberRecord > 1)
            {
                if (string.IsNullOrEmpty(request.ColumnKey))
                {
                    return Json(new ResponseModel<TableDataModel> { Status = 0, Msg = "Please enter column key" });
                }

                var columnKeyName = columns.FirstOrDefault(x => x.Name == request.ColumnKey);
                if (columnKeyName == null)
                {
                    return Json(new ResponseModel<TableDataModel> { Status = 0, Msg = "Column key not found" });
                }

                List<TableDataModel> multiRecord = new();
                for (int i = 1; i <= request.NumberRecord; i++)
                {
                    string newColumnKeyData = CommonHelpers.GenColumnKeyData(int.Parse(columnKeyName.Length), i);
                    var newDataList = existList.Select(x => x == columnKeyData ? newColumnKeyData : x).ToList();

                    TableDataModel newData = new()
                    {
                        DataList = newDataList,
                        Data = CommonHelpers.CombineDataString(newDataList),
                        DataColumn = CommonHelpers.ConvertDataToColumn(newDataList)
                    };

                    multiRecord.Add(newData);
                }

                data.MultiData = multiRecord;
            }

            return Json(new ResponseModel<TableDataModel> { Status = 1, Data = data });
        }

        [HttpPost]
        public async Task<IActionResult> GenTableDataFullLength([FromBody] ColumnRequest request)
        {
            // validate
            if (string.IsNullOrEmpty(request.Input))
            {
                return Json(new ResponseModel<TableDataModel> { Status = 0, Msg = "Please enter the input" });
            }

            TableDataModel data = new();
            List<string> existList = new();

            var columns = CommonHelpers.GetNameAndTypeOfColumn(request.Input);
            if (columns.Any())
            {
                foreach (var column in columns)
                {
                    if (column.Name == Const.SITE_CODE_KEY)
                    {
                        existList.Add(!string.IsNullOrEmpty(request.SiteCode) ? request.SiteCode : Const.DEFAULT_SITE_CODE);
                        continue;
                    }

                    if (_sysVars.Contains(column.Name))
                    {
                        continue;
                    }

                    var manualColumn = request.ManualData.FirstOrDefault(x => x.Name == column.Name);
                    if (manualColumn != null && !string.IsNullOrEmpty(manualColumn.Data))
                    {
                        existList.Add(manualColumn.Data);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(request.ColumnKey) && column.Name == request.ColumnKey)
                    {
                        existList.Add(CommonHelpers.GenColumnKeyData(int.Parse(column.Length), 1, true));
                        continue;
                    }

                    string columnData;
                    string columnType = column.Type;
                    if (columnType == "int" || columnType == "bigint" || columnType == "float" || columnType == "decimal")
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

            existList.AddRange(CommonHelpers.GenSystemData(request.KinoId, request.SystemName));
            data.DataList = existList;
            data.Data = CommonHelpers.CombineDataString(existList);
            data.DataColumn = CommonHelpers.ConvertDataToColumn(existList);

            return Json(new ResponseModel<TableDataModel> { Status = 1, Data = data });
        }
    }
}