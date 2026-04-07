using System;
using _Project.Application.Commands;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Interface for command middleware, which allows for additional processing before and after a command is executed.
    /// This can be used for logging, validation, error handling, or any other cross-cutting concerns related to command execution
    /// </summary>
    public interface ICommandMiddleware
    {
        /// <summary>
        /// Invokes the middleware logic, allowing for processing before and after the command execution. The 'next' parameter
        /// is a delegate that represents the next middleware in the pipeline or the actual command execution if this is the last middleware.
        /// The method returns a CommandResult, which indicates the outcome of the command execution, including success or failure information.
        /// </summary>
        /// <param name="context">The CommandExecutionContext provides information about the command being executed, including the command instance,
        /// validation results, and execution results.</param>
        /// <param name="next">A delegate that represents the next step in the command execution pipeline. This allows the middleware
        /// to control when the next middleware or the command execution itself is invoked.</param>
        /// <returns>A CommandResult indicating the outcome of the command execution, which can include success or failure information.</returns>
        CommandResult Invoke(CommandExecutionContext context, Func<CommandResult> next);
    }
}

