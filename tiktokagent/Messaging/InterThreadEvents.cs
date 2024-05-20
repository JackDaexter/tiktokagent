using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tiktokagent.Domain;

namespace tiktokagent.Messaging;

public enum Author{
    Parent,
    Child
}

public enum ThreadDemand
{
    Run,
    Stop,
    Error,
    ErrorNumber,
    Pause,
    Start,
}

public record InterThreadEvents(Author Author, Account Account, ThreadDemand Status, string StatusMessage = "");


