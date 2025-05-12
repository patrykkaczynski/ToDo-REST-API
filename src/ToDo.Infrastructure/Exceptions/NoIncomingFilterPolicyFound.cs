using ToDo.Application.Enums;
using ToDo.Core.Exceptions;

namespace ToDo.Infrastructure.Exceptions;

public class NoIncomingFilterPolicyFound : CustomException
{
    public NoIncomingFilterPolicyFound(IncomingFilter incomingFilter) 
        : base($"No incoming filter policy found for '{incomingFilter}.")
    {
    }
}