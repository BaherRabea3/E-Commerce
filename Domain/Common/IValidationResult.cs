
namespace Domain.Common
{
    public interface IValidationResult
    {
        public static readonly Error ValidationError = Error.Validation(
            "ValidationError",
            "A validation problme accurred.");

        Error[] Errors { get; }
    }
}
