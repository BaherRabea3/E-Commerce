namespace Domain.Common
{
    public sealed record Error
    {
        public static readonly Error None = new(string.Empty , string.Empty , ErrorType.Failure);


        public Error(string code, string description)
        {
            Code = code;
            Description = description;
        }
        public Error(string code , string description , ErrorType errorType)
        {
            Code = code;
            Description = description;
            Type = errorType;
        }
        public string Code { get; }
        public string Description { get; }
        public ErrorType Type { get; }

        public static Error NotFound(string code , string description)
            => new(code, description, ErrorType.NotFound);
        public static Error Validation(string code, string description)
            => new(code, description, ErrorType.Validation);
        public static Error Failure(string code, string description)
            => new(code, description, ErrorType.Failure);

    }
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2
    }

}
