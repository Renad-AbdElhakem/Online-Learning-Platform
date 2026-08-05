using System.Net;

namespace AssignmentService.Shared
{
    public class GeneralResponse<T>
    {

        public T Data { get; set; }
        public string Message { get; set; }

        public bool IsSuccseded { get; set; }
        public HttpStatusCode? HttpStatusCode { get; set; }

        public static GeneralResponse<T> Success(T data, string? message = null, HttpStatusCode? httpStatusCode = null)
        {

            return new GeneralResponse<T>
            {
                Data = data,
                Message = message,
                IsSuccseded = true,
                HttpStatusCode = httpStatusCode
            };
        }
        public static GeneralResponse<T> Failed(string? errorMessage = null, HttpStatusCode? httpStatusCode = null)
        {

            return new GeneralResponse<T>
            {
                Message = errorMessage,
                IsSuccseded = false,
                HttpStatusCode = httpStatusCode
            };
        }
    }
}
