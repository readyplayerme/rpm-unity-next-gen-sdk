namespace ReadyPlayerMe.Tools.Editor.Data
{
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; }
        
        public string Error { get; set; }
        
        public T Data { get; set; }
    }
}