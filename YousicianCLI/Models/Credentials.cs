namespace YousicianCLI.Models
{
    public readonly struct Credentials
    {
        public readonly string Username;
        public readonly string Password;

        public Credentials(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}