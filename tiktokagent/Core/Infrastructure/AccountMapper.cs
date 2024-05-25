using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tiktokagent.Domain;

namespace tiktokagent.Core.Infrastructure;

public class AccountMapper(string email, string username, string password)
{

    public string Email { get; set; } = email;

    public string Username { get; set; } = username;

    public string Password { get; set; } = password;

   
}
