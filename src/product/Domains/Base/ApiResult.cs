namespace ProductApi.Domains.Base
{
    public class ApiResult
    {
        public List<string> Erros { get; set; } = new List<string>();
        public bool HasError { get { return Erros.Any(); } }
    }

    public class ApiResult<T> : ApiResult
    {
        public T Data { get; set; }
    }
}
