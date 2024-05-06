using tiktokagent.Core.Usecases;
using tiktokagent.Domain;

namespace tiktokagent.Core.Infrastructure;

public class AccountFileAdapter : IObtainAccounts
{
    private string _path;
    
    public AccountFileAdapter(string path)
    {
        _path = path;
    }

    public List<Account> GetAllAccounts()
    {
        throw new NotImplementedException();
    }

    public List<AccountProxied> GetAccountsProxieds()
    {
        throw new NotImplementedException();
    }

    public void SaveAccounts(Account accountProxied)
    {
        throw new NotImplementedException();
    }


}