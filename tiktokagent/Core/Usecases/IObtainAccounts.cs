using tiktokagent.Domain;

namespace tiktokagent.Core.Usecases;

public interface IObtainAccounts
{
    public List<Account> GetAllAccounts();
    public List<AccountProxied> GetAccountsProxieds();
    public void SaveAccounts(Account account);
}