using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCommand : ICommand
{
    IshootStrategy _storedStrategy;
    public ShootCommand(IshootStrategy strategy)
    {
        this._storedStrategy = strategy;
    }

    public void Execute()
    {
        _storedStrategy.Shoot();
    }

    public void Undo()
    {
        _storedStrategy.Undo();
    }
}
