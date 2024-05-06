using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tiktokagent.Domain;

namespace tiktokagent.Core.Streaming;

public class MainBot
{
    public Account _account;
    public AccountProxied _accountProxied;

    public MainBot(Account account)
    {
        _account = account;
        Initialize();
    }

    private void Initialize()
    {
        throw new NotImplementedException();
    }
}
