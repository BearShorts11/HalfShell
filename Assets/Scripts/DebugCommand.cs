using System;
using System.Collections.Generic;
using UnityEngine;


public class DebugCommandBase
{
    private string commandId;
    private string commandDescription;
    private string commandFormat;

    public string CommandId { get { return commandId; } }
    public string CommandDescription { get { return commandDescription; } }
    public string CommandFormat { get { return commandFormat; } }

    public DebugCommandBase(string id, string description, string format)
    { 
        commandId = id;
        commandDescription = description;
        commandFormat = format;
    }
}


public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(string id, string description, string format, Action command) : base(id, description, format)
    {
        this.command = command;
    }

    public void Invoke()
    { 
        this.command.Invoke();
    }
}

public class DebugCommand<T1> : DebugCommandBase
{
    private Action<T1> command;

    public DebugCommand(string id, string description, string format, Action<T1> command) : base(id, description, format)
    {
        this.command = command;
    }

    public void Invoke(T1 value)
    {
        this.command.Invoke(value);
    }
}

