using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AddictedProxy.Controllers.Rest;

[Route("application")]
public class ApplicationController : Controller
{
    public record ApplicationInfoDto(string ApplicationVersion)
    {
        /// <summary>
        /// Version of the application
        /// </summary>
        /// <example>2.9.5</example>
        [Required]
        public string ApplicationVersion { get; init; } = ApplicationVersion;
    }

    /// <summary>
    /// Information about the application
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("info")]
    [ResponseCache(Duration = 2 * 3600)]
    [Produces("application/json")]
    public Ok<ApplicationInfoDto> Version()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        return TypedResults.Ok(new ApplicationInfoDto(executingAssembly.GetName().Version!.ToString(3)));
    }
}