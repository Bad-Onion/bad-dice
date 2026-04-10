using _Project.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _Project.Application.Commands
{
    /// <summary>
    /// CommandProcessor is responsible for executing commands that implement the ICommand interface. It processes commands
    /// through a pipeline of middlewares, allowing for cross-cutting concerns such as logging, validation, and error handling to be
    /// applied consistently across all commands. The processor validates the command before execution and returns a CommandResult indicating
    /// the success or failure of the command execution.
    /// </summary>
    public class CommandProcessor
    {
        private readonly IReadOnlyList<ICommandMiddleware> _middlewares;

        public CommandProcessor(IEnumerable<ICommandMiddleware> middlewares = null)
        {
            _middlewares = (middlewares ?? Array.Empty<ICommandMiddleware>()).ToList();
        }

        public CommandResult ExecuteCommand(ICommand command)
        {
            if (command == null)
            {
                return CommandResult.Failure("NullCommand", "Cannot execute a null command instance.");
            }

            CommandExecutionContext context = new CommandExecutionContext(command);

            Func<CommandResult> pipeline = () => ExecuteCore(context);
            for (int index = _middlewares.Count - 1; index >= 0; index--)
            {
                ICommandMiddleware middleware = _middlewares[index];
                Func<CommandResult> next = pipeline;
                pipeline = () => middleware.Invoke(context, next);
            }

            return pipeline();
        }

        private static CommandResult ExecuteCore(CommandExecutionContext context)
        {
            ValidationResult validationResult = context.Command.Validate();
            context.ValidationResult = validationResult;

            if (!validationResult.IsValid)
            {
                context.Result = CommandResult.Invalid(validationResult);
                return context.Result;
            }

            context.Result = context.Command.Execute();
            return context.Result;
        }
    }
}