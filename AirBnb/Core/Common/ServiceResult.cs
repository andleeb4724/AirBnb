namespace AirBnb.Core.Common
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? Error { get; private set; }
        public int StatusCode { get; private set; }

        public static ServiceResult<T> Ok(T data) =>
            new() { IsSuccess = true, Data = data, StatusCode = 200 };

        public static ServiceResult<T> Fail(string error, int statusCode = 400) =>
            new() { IsSuccess = false, Error = error, StatusCode = statusCode };

        public static ServiceResult<T> NotFound(string error) =>
            Fail(error, 404);

        public static ServiceResult<T> Forbidden(string error) =>
            Fail(error, 403);

        public static ServiceResult<T> Unauthorized(string error) =>
            Fail(error, 401);
    }
}
