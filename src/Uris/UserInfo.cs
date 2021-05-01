namespace Urls
{
    /// <summary>
    /// Url credentials. Warning: using this is not recommended. This is here for completeness
    /// </summary>
    public record UserInfo
    (
        string Username,
        string Password
    )
    {
        #region Public Properties
        public static UserInfo Empty { get; } = new("", "");
        #endregion

        #region Constructors
        public UserInfo(UserInfo userInfo)
        {
            userInfo ??= Empty;

            Username = userInfo.Username;
            Password = userInfo.Password;
        }
        #endregion

        #region Public Methods
        public override string ToString()
            => $"{(!string.IsNullOrEmpty(Username) ? $"{Username}:{Password}@" : "")}";
        #endregion
    }
}

