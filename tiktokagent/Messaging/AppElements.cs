using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tiktokagent.Messaging;

public enum InteractionStatus
{
   ErrorNumberOfAccounts,
}

public record AppElements(InteractionStatus Status, string StatusMessage = "");
