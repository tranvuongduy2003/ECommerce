namespace Shared.SeedWork.ApiResult;

public class ApiSuccessResult<T> : ApiResult<T>
{
    public ApiSuccessResult(T data) : base(true, data, "Success")
    {
    }
}