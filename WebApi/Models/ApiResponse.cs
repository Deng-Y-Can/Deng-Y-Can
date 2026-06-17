using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApi.Models
{
    public class ApiResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; } = 200;

        [JsonProperty("message")]
        public string Message { get; set; } = "success";

        [JsonProperty("data")]
        public object Data { get; set; }

        public static ApiResponse Success(object data = null, string message = "success")
        {
            return new ApiResponse { Code = 200, Message = message, Data = data };
        }

        public static ApiResponse Fail(string message, int code = 500)
        {
            return new ApiResponse { Code = code, Message = message, Data = null };
        }
    }

    public class ApiResponse<T>
    {
        [JsonProperty("code")]
        public int Code { get; set; } = 200;

        [JsonProperty("message")]
        public string Message { get; set; } = "success";

        [JsonProperty("data")]
        public T Data { get; set; }

        public static ApiResponse<T> Success(T data, string message = "success")
        {
            return new ApiResponse<T> { Code = 200, Message = message, Data = data };
        }

        public static ApiResponse<T> Fail(string message, int code = 500)
        {
            return new ApiResponse<T> { Code = code, Message = message, Data = default };
        }
    }

    public class PagedResult<T>
    {
        [JsonProperty("items")]
        public List<T> Items { get; set; } = new List<T>();

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 0;
    }
}
