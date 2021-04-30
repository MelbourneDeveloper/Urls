using System;

namespace Urls
{
    public record UserInfo
    (
        string Username,
        string Password
    )
    {
        public static UserInfo Empty { get; } = new("", "");

        public UserInfo(UserInfo userInfo)
        {
            if (userInfo == null) throw new ArgumentNullException(nameof(userInfo));

            Username = userInfo.Username;
            Password = userInfo.Password;
        }

        public override string ToString()
            => $"{(!string.IsNullOrEmpty(Username) ? $"{Username}:{Password}@" : "")}";
    }
}

