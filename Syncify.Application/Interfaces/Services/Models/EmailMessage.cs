namespace Syncify.Application.Interfaces.Services.Models;
public class EmailMessage : BaseEmailMessage
{
    public string To { get; set; } = null!;
}