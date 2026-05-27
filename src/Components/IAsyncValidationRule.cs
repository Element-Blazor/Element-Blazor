using System.Threading.Tasks;

namespace Element
{
    public interface IAsyncValidationRule : IValidationRule
    {
        Task<bool> ValidateAsync(object value);
    }
}
