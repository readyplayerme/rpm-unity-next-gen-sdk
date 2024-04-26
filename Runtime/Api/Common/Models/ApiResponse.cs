namespace ReadyPlayerMe.Runtime.Api.Common.Models
{
    public abstract class ApiResponse
    {
        public bool IsSuccess { get; set; } = true;

        public long Status { get; set; } = 200;
        
        public string Error { get; set; }
    }
}