namespace AddictedProxy.Model.Config;

public class Addic7edCreds
{
    public Addic7edCreds(long userId, string password)
    {
        UserId = userId;
        Password = password;
    }

    public long UserId { get; set; }
    public string Password { get; set; }
}