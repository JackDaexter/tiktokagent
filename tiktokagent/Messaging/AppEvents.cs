using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tiktokagent.Messaging;

public enum ApplicationEvents
{
   ErrorNumberOfAccounts,
   AccountLoaded,
   AccountSaved,
   AccountAdded,
   AccountRemoved,
   AccountAlreadyExist,
   SelectAccountToRemove,
   FilePathError
}

public record AppEvents(ApplicationEvents Status, string StatusMessage = "");
