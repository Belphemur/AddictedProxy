using AntiCaptcha.Model.Error;
using AntiCaptcha.Model.Task;
using AntiCaptcha.Model.Task.Turnstile;

namespace AntiCaptcha.Service;

public interface IAntiCaptchaClient
{
    /// <summary>
    /// Solve a Turnstile task using AntiCaptcha Proxyless API
    /// </summary>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="SolvingException">If couldn't solve the captcha</exception>
    Task<SolutionResponse<TurnstileSolution>?> SolveTurnstileProxylessAsync(TurnstileProxylessTask task, CancellationToken cancellationToken = default);
}