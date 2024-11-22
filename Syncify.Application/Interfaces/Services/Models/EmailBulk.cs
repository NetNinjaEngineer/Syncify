namespace Syncify.Application.Interfaces.Services.Models;

public sealed class EmailBulk : BaseEmailMessage
{
    public List<string> ToReceipients { get; set; } = [];
}