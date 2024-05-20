using tiktokagent.Domain;

namespace tiktokagent.Core.Usecases;

public interface IObtainAccounts
{
    public Task<List<Account>> LoadAllAccountsAsync();
    public Task SaveMultipleAccounts(List<Account> account);
    public Task SaveAccount(Account account);
}