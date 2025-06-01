namespace ToDo.Core.Exceptions;

public sealed class EmptyDescriptionException() : CustomException("The description cannot be empty or whitespace.");