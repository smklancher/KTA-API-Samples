using Agility.Sdk.Model.Capture;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TotalAgility.Sdk;

namespace KtaCase
{
    public class Users
    {
        public string LoggedOnUserNameAndSessionCsv(string sessionId)
        {
            var us = new UserService();
            var users = us.GetLoggedOnUsers(sessionId);
            var str = string.Join("\r\n", users.Select(u => $"{u.UserId}, {u.SessionId}"));
            return $"username, sessionID\r\n{str}";
        }
    }
}