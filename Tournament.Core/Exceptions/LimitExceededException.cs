using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Exceptions;

namespace Tournament.Core.Exceptions
{
    public abstract class LimitExceededException : Exception
    {
        public string Title { get; }
        protected LimitExceededException(string message, string title = "Limit exceeded") : base(message)
        {
            Title = title;
        }
    }
}


public class GameLimitExceededException : LimitExceededException
{
    public GameLimitExceededException(string title) : base($"Game limit exceeded for tournament {title}") { }
}