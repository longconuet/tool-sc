namespace ToolSC.Models
{
    public class ResponseModel
    {
        public int Status {  get; set; }
        public string Msg {  get; set; }
    }

    public class ResponseModel<T>
    {
        public int Status { get; set; }
        public string Msg { get; set; }
        public T? Data { get; set; }
    }
}
