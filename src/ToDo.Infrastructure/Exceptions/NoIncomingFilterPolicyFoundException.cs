using ToDo.Application.Enums;
using ToDo.Core.Exceptions;

namespace ToDo.Infrastructure.Exceptions;

public class NoIncomingFilterPolicyFoundException : CustomException
{
    public NoIncomingFilterPolicyFoundException(IncomingFilter incomingFilter) 
        : base($"No incoming filter policy found for '{incomingFilter}.")
    {
    }
}