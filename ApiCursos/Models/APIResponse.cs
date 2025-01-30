using System.Net;

namespace ApiCursos.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
            IsSuccess = false;
            Result = new();
        }

        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; }

        public List<string> ErrorMessages { get; set; }

        public object Result { get; set; }
    }
}