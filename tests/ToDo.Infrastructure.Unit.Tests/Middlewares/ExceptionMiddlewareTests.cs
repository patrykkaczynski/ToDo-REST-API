using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using ToDo.Application.Exceptions;
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
        var middleware = new ExceptionMiddleware();
        
        // Act
        
        await middleware.InvokeAsync(context, nextDelegateMock.Object);
        
        // Assert
        
        nextDelegateMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
    }
    
    [Fact]
    public async Task InvokeAsync_WhenCustomExceptionIsThrown_ShouldSetStatusCode400()
    {
        // Arrange

        var context = new DefaultHttpContext();
        var middleware = new ExceptionMiddleware();
        var id = Guid.NewGuid();
        var customException = new ToDoTaskNotFoundException(id);
        
        // Act
        
        await middleware.InvokeAsync(context, _ => throw customException);
        
        // Assert
        
        context.Response.StatusCode.ShouldBe(400);
    }
    
    [Fact]
    public async Task InvokeAsync_WhenGenericExceptionIsThrown_ShouldSetStatusCode500()
    {
        // Arrange

        var context = new DefaultHttpContext();
        var middleware = new ExceptionMiddleware();
        var exception = new Exception();
        
        // Act
        
        await middleware.InvokeAsync(context, _ => throw exception);
        
        // Assert
        
        context.Response.StatusCode.ShouldBe(500);
    }
}