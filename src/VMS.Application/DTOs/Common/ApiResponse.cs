namespace VMS.Application.DTOs.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public string? ErrorCode { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T> { Success = true, Message = message, Data = data };
    }

    public static ApiResponse<T> FailResponse(string message, string? errorCode = null)
    {
        return new ApiResponse<T> { Success = false, Message = message, ErrorCode = errorCode };
    }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }

    public static ApiResponse SuccessResponse(string message = "Success")
    {
        return new ApiResponse { Success = true, Message = message };
    }

    public static ApiResponse FailResponse(string message, string? errorCode = null)
    {
        return new ApiResponse { Success = false, Message = message, ErrorCode = errorCode };
    }
}
