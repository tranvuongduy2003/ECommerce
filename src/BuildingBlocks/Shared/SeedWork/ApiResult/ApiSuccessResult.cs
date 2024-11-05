using System.Text.Json.Serialization;

namespace Shared.SeedWork.ApiResult;

public class ApiSuccessResult<T> : ApiResult<T>
{
    [JsonConstructor]
    public ApiSuccessResult(T data) : base(true, data, "Success")
    {
    }
}