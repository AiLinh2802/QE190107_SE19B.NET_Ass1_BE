namespace TaskTrack.Service.Exceptions;

public class ServiceException(int statusCode, string message, IDictionary<string, string[]>? errors = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public IDictionary<string, string[]>? Errors { get; } = errors;

    public static ServiceException NotFound(string resource, int id) =>
        new(404, $"{resource} with ID {id} was not found.");

    public static ServiceException BadRequest(string field, string message) =>
        new(400, message, new Dictionary<string, string[]> { [field] = [message] });
}
