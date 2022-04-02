namespace AddictedProxy.Model.Config
{
    public class Addic7edCreds
    {
        public long UserId { get; set; }
        public string Password { get; set; }

        public Addic7edCreds(long userId, string password)
        {
            UserId = userId;
            Password = password;
        }
    }
}