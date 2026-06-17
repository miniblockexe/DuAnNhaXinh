namespace NhaXinh.DTOs
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ApiResponseDto<T> Ok(T data, string message = "Thành công")
            => new() { Success = true, Message = message, Data = data };

        public static ApiResponseDto<T> Fail(string message)
            => new() { Success = false, Message = message, Data = default };
    }
    public class ApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public static ApiResponseDto Ok(string message = "Thành công")
            => new() { Success = true, Message = message };

        public static ApiResponseDto Fail(string message)
            => new() { Success = false, Message = message };
    }
}
