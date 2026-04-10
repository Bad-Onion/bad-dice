namespace _Project.Application.Commands
{
    /// <summary>
    /// Represents the result of validating a command. It indicates whether the command is valid and provides error information if it is not.
    /// </summary>
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

