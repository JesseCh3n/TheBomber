using System.Collections;
using System.Collections.Generic;

public class CommandInvoker
{
    Stack<ICommand> _commandList;

    public CommandInvoker()
    {
        _commandList = new Stack<ICommand>();
    }

    public void AddCommand(ICommand newCommand)
    {
        newCommand.Execute();
        _commandList.Push(newCommand);
    }

    public void UndoCommand()
    {
        if (_commandList.Count > 0)
        {
            ICommand latestCommand = _commandList.Pop();
            latestCommand.Undo();
        }
    }

    public void ClearCommand()
    {
        _commandList.Clear();
    }
}
