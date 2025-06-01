namespace ToDo.Core.Exceptions;

public sealed class EmptyTitleException() : CustomException("The title cannot be empty or whitespace.");