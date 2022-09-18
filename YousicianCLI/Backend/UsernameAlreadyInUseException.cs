using System;

namespace YousicianCLI.Backend
{
    public class UsernameAlreadyInUseException : Exception
    {
        public readonly string Username;

        public UsernameAlreadyInUseException(string username)
            : base($"Username '{username}' already in use")
        {
            Username = username;
        }
    }
}