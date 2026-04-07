using System;
using System.Diagnostics;
using _Project.Application.Interfaces;
using UnityDebug = UnityEngine.Debug;

namespace _Project.Application.Commands
{
    /// <summary>
    /// CommandLoggingMiddleware is a middleware component for the CommandProcessor that logs the execution of commands.
    /// It measures the time taken to execute each command and logs whether the command succeeded or failed, along with any
    /// relevant messages or exceptions. This helps in monitoring the performance and behavior of commands within the application.
    /// </summary>
    public class CommandLoggingMiddleware : ICommandMiddleware
    {
        public CommandResult Invoke(CommandExecutionContext context, Func<CommandResult> next)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                CommandResult result = next();
                stopwatch.Stop();

                if (result.IsSuccess)
                {
                    UnityDebug.Log($"Command '{context.Command.GetType().Name}' completed in {stopwatch.ElapsedMilliseconds} ms.");
                }
                else
                {
                    UnityDebug.LogWarning($"Command '{context.Command.GetType().Name}' failed ({result.ErrorCode}): {result.Message}");
                }

                return result;
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                UnityDebug.LogError($"Command '{context.Command.GetType().Name}' threw after {stopwatch.ElapsedMilliseconds} ms: {exception}");
                return CommandResult.Failure("CommandException", exception.Message);
            }
        }
    }
}


