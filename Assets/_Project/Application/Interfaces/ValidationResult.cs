namespace _Project.Application.Interfaces
{
    public readonly struct ValidationResult
    {
        public bool IsValid { get; }
        public string ErrorCode { get; }
        public string Message { get; }

        private ValidationResult(bool isValid, string errorCode, string message)
        {
            IsValid = isValid;
            ErrorCode = errorCode;
            Message = message;
        }

        public static ValidationResult Success()
        {
            return new ValidationResult(true, string.Empty, string.Empty);
        }

        public static ValidationResult Failure(string errorCode, string message)
        {
            return new ValidationResult(false, errorCode ?? "ValidationFailed", message ?? string.Empty);
        }
    }
}

