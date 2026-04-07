using System;
using System.Diagnostics;
using _Project.Application.Commands;
using _Project.Application.Interfaces;
using UnityDebug = UnityEngine.Debug;

namespace _Project.Infrastructure.Features.Commands
{
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


