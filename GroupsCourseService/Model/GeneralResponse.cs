using System.Net;
using System.Net.Mail;

namespace GroupsCourseService.Model
{
    public class GeneralResponse<T>
    {
        public T Data { get; set; }
        public bool IsSuccseded { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public HttpStatusCode? HttpStatusCode { get; set; }


        public static GeneralResponse<T> Succsess(T data, string? message=null, HttpStatusCode? httpStatusCode = null)
        {

            return new GeneralResponse<T>
            {
                Data = data,
                Message = message,
                IsSuccseded = true,
                HttpStatusCode = httpStatusCode
            };
        }
        public static GeneralResponse<T> failed(string? errorMessage=null,HttpStatusCode? httpStatusCode = null)
        {

            return new GeneralResponse<T>
            {
                ErrorMessage = errorMessage,
                IsSuccseded = false,
                HttpStatusCode = httpStatusCode
            };
        }

    }
}
