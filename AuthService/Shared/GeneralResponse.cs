using System.Net;

namespace AuthService.Shared
{
    public class GeneralResponse<T>
    {
        public T Data { get; set; }
        public string ?Message { get; set; }
        public bool IsSuccseded { get; set; }
      

        public static GeneralResponse<T> Success(T data, string? message = null)
        {

            return new GeneralResponse<T>
            {
                Data = data,
                Message = message,
                IsSuccseded = true,
               
            };
        }
        public static GeneralResponse<T> Failed(string? errorMessage = null)
        {

            return new GeneralResponse<T>
            {
                Message = errorMessage,
                IsSuccseded = false,
         
            };
        }
    }
}
