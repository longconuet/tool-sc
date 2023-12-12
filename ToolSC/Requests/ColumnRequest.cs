namespace ToolSC.Requests
{
    public class ColumnRequest
    {
        public string Input {  get; set; }
        public string SiteCode {  get; set; }
        public string KinoId {  get; set; }
        public string ColumnKey {  get; set; }
        public int NumberRecord { get; set; } = 1;
        public string TableName {  get; set; }
        public string SystemName {  get; set; }
        public List<ManualDataRequest> ManualData { get; set; } = new List<ManualDataRequest>();
    }

    public class ManualDataRequest
    {
        public string Name { get; set; }
        public string Data { get; set; }
    }
}
