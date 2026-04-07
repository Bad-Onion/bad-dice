using System;
using _Project.Application.Commands;

namespace _Project.Application.Interfaces
{
    public interface ICommandMiddleware
    {
        CommandResult Invoke(CommandExecutionContext context, Func<CommandResult> next);
    }
}

