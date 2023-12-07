namespace ToolSC.Models
{
    public class TableDataModel
    {
        public string Data {  get; set; }
        public string DataColumn {  get; set; }
        public List<string> DataList {  get; set; }
        public List<TableDataModel> MultiData {  get; set; }
    }
}
