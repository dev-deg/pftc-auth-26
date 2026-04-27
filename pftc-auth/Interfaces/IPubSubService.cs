namespace pftc_auth.Interfaces;

public interface IPubSubService
{
    Task PublishMessageAsync(string message);
}