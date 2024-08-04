namespace KloneApi.SharedDomain;

public class BaseResult<T>: BaseResult
{
    public T? Data { get; set; }
}

public class BaseResult
{
    public bool IsSuccess { get; set; }

    public string? Message { get; set; }
}
