using System.Net;

namespace HMS.Application.Models;

public class ApiResponse<T>
{
    public string Message { get; set; } = string.Empty;
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public T? Result { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success", HttpStatusCode statusCode = HttpStatusCode.OK, bool isSuccess = true) =>
        new() { IsSuccess = isSuccess, Message = message, Result = data, StatusCode = statusCode };

    public static ApiResponse<T> Fail(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, bool isSuccess = false) =>
        new() { IsSuccess = isSuccess, Message = message, StatusCode = statusCode };
}
