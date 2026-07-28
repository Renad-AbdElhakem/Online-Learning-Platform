namespace CourseService.Model
{
    public  class GeneralResponse<T>
    {

        public T Data { get; set; }
        public string Message { get; set; }

        public bool IsSucceeded { get; set; }




        public static GeneralResponse<T> Successful(T data,string message)
        {

            return new GeneralResponse<T>
            {
                Data = data,
                Message = message ,
                IsSucceeded = true
                
            };

        }
        public static GeneralResponse<T> Failed(string message)
        {

            return new GeneralResponse<T>
            {
               
                Message = message ,
                IsSucceeded = false
                
            };

        }






    }
}
