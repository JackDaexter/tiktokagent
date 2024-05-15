using tiktokagent.Domain;

namespace tiktokagent.Core.Usecases;

public class AccountManager
{
    private readonly Random _random = new Random();
    private IObtainAccounts _repository;

    public AccountManager(IObtainAccounts repository)
    {
        _repository = repository;
    }

    
    public List<Account> GenerateAccounts(int numberOfAccounts)
    {
        var accounts = new List<Account>();
        for (var i = 0; i < numberOfAccounts; i++)
        {
            var accountWithoutProxy = GenerateAccountWithoutProxy();
            accounts.Add(accountWithoutProxy);
        }
        _repository.SaveMultipleAccounts(accounts);

        return accounts;
    }
    
    public List<Account> GetExistingAccounts(int numberOfAccounts)
    {
        var accounts = _repository.LoadAllAccounts();
        return accounts.Count >= numberOfAccounts ? accounts.Take(numberOfAccounts).ToList() : accounts;
    } 
    
    
    private Account GenerateAccountWithoutProxy()
    {
        var username = RandomUsername();
        var email = EmailFromUsername(username);
        var fixedPass = FixedPassword();
        var account = new Account(email,username, fixedPass, Status.Inactive); 
        return account;
    }
    
    
    private string EmailFromUsername(string username)
    {   
        return username + "@tomistopate.eu";
    }

    private string RandomUsername()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var accountSize = _random.Next(5, 15);
        
        return new string(Enumerable.Repeat(chars, accountSize)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }


    private string FixedPassword()
    {
        return "sC224t0d$^*";
    }
}