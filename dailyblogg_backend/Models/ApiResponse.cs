namespace dailyblogg_backend.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }

        // Helper method for Success
        public static ApiResponse<T> SuccessResult(T data) =>
            new ApiResponse<T> { Success = true, Data = data };

        // Helper method for Failure
        public static ApiResponse<T> FailureResult(string message) =>
            new ApiResponse<T> { Success = false, Error = message };
    }
}
