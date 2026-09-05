using Microsoft.AspNetCore.Identity;

namespace DemoBoard_server.Exceptions;

public class InvalidNewPasswordException : Exception
{
    public InvalidNewPasswordException(IEnumerable<IdentityError> errors)
    {
        Errors = errors;
    }

    public IEnumerable<IdentityError> Errors { get; }
}