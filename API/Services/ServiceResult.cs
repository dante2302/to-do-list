namespace API.Services;

public class ServiceResult<T>(bool isSuccess, T? data = default, string? message = null)
{
    public bool IsSuccess { get; set; } = isSuccess;
    public T? Data { get; set; } = data;
    public string? Message { get; set; } = message;

    public static ServiceResult<T> Success(T data) => new(true, data);
    public static ServiceResult<T> Failure(string message) => new(false, message: message);
}