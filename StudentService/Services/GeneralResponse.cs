using System.Net;

namespace StudentService.Services
{
    public class GeneralResponse<T>
    {
        public T Data { get; set; }
        public bool IsSuccseded { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
     


        public static GeneralResponse<T> Succsess(T data, string? message = null)
        {

            return new GeneralResponse<T>
            {
                Data = data,
                Message = message,
                IsSuccseded = true,
           
            };
        }
        public static GeneralResponse<T> failed(string? errorMessage = null)
        {

            return new GeneralResponse<T>
            {
                ErrorMessage = errorMessage,
                IsSuccseded = false,

            };
        }
    }
}
