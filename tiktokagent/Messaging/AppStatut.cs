using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tiktokagent.Messaging;

public enum ApplicationStatus
{
    Running,
    Stopped,
    Error,
    ErrorNumber,
    Paused,
    Starting,
    NotStarted
}

public record AppStatus(ApplicationStatus Status, string StatusMessage="");
