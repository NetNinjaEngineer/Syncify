namespace Syncify.Application.Interfaces.Services.Models;
public abstract class BaseEmailMessage
{
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
}
