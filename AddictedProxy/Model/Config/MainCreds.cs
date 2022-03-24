using AngleSharp.Text;

namespace AddictedProxy.Model.Config;

public class MainCreds : Addic7edCreds
{
    public override long UserId => Environment.GetEnvironmentVariable("ADDICTED_USERID").ToInteger(0);
    public override string Password => Environment.GetEnvironmentVariable("ADDICTED_PASS") ?? throw new InvalidOperationException("No main password");
}