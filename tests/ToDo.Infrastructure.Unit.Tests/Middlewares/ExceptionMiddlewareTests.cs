using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using ToDo.Application.Common;
using ToDo.Application.Exceptions;
using ToDo.Core.Exceptions;
using ToDo.Infrastructure.Exceptions;
using ToDo.Infrastructure.Middlewares;

namespace ToDo.Infrastructure.Unit.Tests.Middlewares;

public class ExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenNoExceptionIsThrown_ShouldCallNextDelegate()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var nextDelegateMock = new Mock<RequestDelegate>();
        var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
        var middleware = new ExceptionMiddleware(loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context, nextDelegateMock.Object);

        // Assert
        nextDelegateMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
    }

    [Theory]
    [MemberData(nameof(ExceptionTestData))]
    public async Task InvokeAsync_WhenExceptionIsThrown_ShouldSetExpectedStatusCode(Exception exception,
        int expectedStatusCode)
    {
        // Arrange
        var context = new DefaultHttpContext();
        var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
        var middleware = new ExceptionMiddleware(loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context, _ => throw exception);

        // Assert
        context.Response.StatusCode.ShouldBe(expectedStatusCode);
    }

    public static IEnumerable<object[]> ExceptionTestData =>
        new List<object[]>
        {
            new object[] { new EmptyDescriptionException(), HttpStatusCode.BadRequest },
            new object[] { new EmptyTitleException(), HttpStatusCode.BadRequest },
            new object[] { new InvalidDescriptionException("D"), HttpStatusCode.BadRequest },
            new object[] { new InvalidEntityIdException(Guid.NewGuid()), HttpStatusCode.BadRequest },
            new object[]
                { new InvalidExpirationDateException(DateTime.UtcNow.AddDays(-10)), HttpStatusCode.BadRequest },
            new object[] { new InvalidPercentCompleteException(-20), HttpStatusCode.BadRequest },
            new object[] { new InvalidTitleException("T"), HttpStatusCode.BadRequest },
            new object[] { new ToDoTaskNotFoundException(Guid.NewGuid()), HttpStatusCode.NotFound },
            new object[] { new InvalidPageNumberException(12, 11), HttpStatusCode.BadRequest },
            new object[] { new InvalidPageSizeException(7), HttpStatusCode.BadRequest },
            new object[] { new InvalidSortByColumnNameException(), HttpStatusCode.BadRequest },
            new object[]
                { new NoIncomingFilterPolicyFoundException(IncomingFilter.None), HttpStatusCode.NotFound },
            new object[] { new Exception("Unexpected"), HttpStatusCode.InternalServerError }
        };
}