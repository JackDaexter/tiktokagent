using tiktokagent.Domain;

namespace tiktokagent.Core.Usecases;

public interface IObtainAccounts
{
    public List<Account> LoadAllAccounts();
    public List<AccountProxied> GetAccountsProxieds();
    public void SaveMultipleAccounts(List<Account> account);
    public void SaveAccount(Account account);
}