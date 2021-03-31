namespace Uris
{
    public record UserInfo
    (
        string Username,
        string Password
    )
    {
        public override string ToString()
            => $"{(!string.IsNullOrEmpty(Username) ? $"{Username}:{Password}@" : "")}";
    }
}

