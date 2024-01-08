using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.InplayMatrix.Controllers;

[ApiController]
[Route("im_esp")]
public class IMESPController : ControllerBase
{
    /// <summary>
    /// Executes the asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "reportUrl":"http://operatorapi-report.staging.imaegisapi.com/",
    ///             "merchantCode":"OfL5H2hKOIP9tHp9ZIoqYeggJtFC39zQ",
    ///             "merchantPrefix":"IM1S3",
    ///             "startTime": "2023-04-27T13:00:00+08:00",
    ///             "endTime": "2023-04-27T14:00:00+08:00"
    ///         }
    /// </remarks>
    [HttpPost]
    public ValueTask ExecuteAsync(
        [FromServices] ICommandService<PullRecordByIMESportsCommand> service,
        PullRecordByIMESportsCommand source)
        => service
        .ExecuteAsync(
            command: source,
            cancellationToken: HttpContext.RequestAborted);
}
