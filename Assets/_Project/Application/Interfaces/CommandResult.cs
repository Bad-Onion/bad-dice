namespace _Project.Application.Interfaces
{
    public readonly struct CommandResult
    {
        public bool IsSuccess { get; }
        public string ErrorCode { get; }
        public string Message { get; }

        private CommandResult(bool isSuccess, string errorCode, string message)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            Message = message;
        }

        public static CommandResult Success()
        {
            return new CommandResult(true, string.Empty, string.Empty);
        }

        public static CommandResult Invalid(ValidationResult validationResult)
        {
            return new CommandResult(false, validationResult.ErrorCode, validationResult.Message);
        }

        public static CommandResult Failure(string errorCode, string message)
        {
            return new CommandResult(false, errorCode ?? "CommandFailed", message ?? string.Empty);
        }
    }
}

