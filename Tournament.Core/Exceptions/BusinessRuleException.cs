using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Exceptions;

public abstract class BusinessRuleException : Exception
{
    public string Title { get; set; }

    protected BusinessRuleException(string message, string title = "Business Rule Exception") : base(message)
    {
        Title = title;
    }
}

public class GameLimitReachedException : BusinessRuleException
{
    public GameLimitReachedException(int maxGameCount) : base($"Maximum {maxGameCount} games per tournament allowed.", "Max Game Limit Reached")
    {
    }
}