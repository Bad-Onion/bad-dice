namespace _Project.Application.Commands
{
    /// <summary>
    /// The CommandResult struct represents the outcome of executing a command. It indicates whether the command was successful,
    /// and if not, it provides an error code and message to describe the failure. This struct is used throughout the command processing
    /// pipeline to standardize how results are communicated back to the caller and any middleware that may be involved in handling the command execution.
    /// </summary>
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

