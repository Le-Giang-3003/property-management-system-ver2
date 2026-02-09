namespace PropertyManagementSystemVer2.BLL.DTOs
{
    public class ServiceResultDto<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ServiceResultDto<T> Success(T data, string? message = null)
        {
            return new ServiceResultDto<T> { IsSuccess = true, Data = data, Message = message };
        }

        public static ServiceResultDto<T> Failure(string error)
        {
            return new ServiceResultDto<T> { IsSuccess = false, Errors = new List<string> { error } };
        }

        public static ServiceResultDto<T> Failure(List<string> errors)
        {
            return new ServiceResultDto<T> { IsSuccess = false, Errors = errors };
        }
    }

    public class ServiceResultDto
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ServiceResultDto Success(string? message = null)
        {
            return new ServiceResultDto { IsSuccess = true, Message = message };
        }

        public static ServiceResultDto Failure(string error)
        {
            return new ServiceResultDto { IsSuccess = false, Errors = new List<string> { error } };
        }

        public static ServiceResultDto Failure(List<string> errors)
        {
            return new ServiceResultDto { IsSuccess = false, Errors = errors };
        }
    }
}
