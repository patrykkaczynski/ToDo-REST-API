using ToDo.Application.Common;
using ToDo.Core.Exceptions;

namespace ToDo.Infrastructure.Exceptions;

public class NoIncomingFilterPolicyFoundException(IncomingFilter incomingFilter)
    : CustomException($"No incoming filter policy found for '{incomingFilter}.");