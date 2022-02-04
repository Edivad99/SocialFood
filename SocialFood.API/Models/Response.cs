namespace SocialFood.API.Models;


public class Response
{
    public int StatusCode { get; set; }
}

public class Response<T> : Response
{
    public T? Result { get; set; }
}
