using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Framework.Core.Server.Controllers;

[Route("api/framework/core/sms-webhook")]
public class SmsWebhookController(
    ILogger<SmsWebhookController> logger,
    ISmsWebhookService smsWebhookService)
    : ControllerBase
{
    [HttpPost("twilio/{channelKey}/message")]
    [HttpPost("twilio/message")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<ActionResult> ReceiveTwilioMessage(String? channelKey = null)
    {
        var request = await BuildRequest(channelKey);
        var response = await smsWebhookService.ReceiveInboundMessage(new(request));

        if (!response.Ok)
        {
            logger.LogWarning("Twilio inbound SMS webhook rejected. {errors}", response.ErrorMessages);
            return BadRequest();
        }

        return Content("<Response></Response>", "text/xml");
    }

    [HttpPost("twilio/{channelKey}/status")]
    [HttpPost("twilio/status")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<ActionResult> ReceiveTwilioStatus(String? channelKey = null)
    {
        var request = await BuildRequest(channelKey);
        var response = await smsWebhookService.ReceiveStatusCallback(new(request));

        if (!response.Ok)
        {
            logger.LogWarning("Twilio SMS status webhook rejected. {errors}", response.ErrorMessages);
            return BadRequest();
        }

        return Ok();
    }

    private async Task<TwilioSmsWebhookRequest> BuildRequest(String? channelKey)
    {
        var form = await Request.ReadFormAsync();

        return new()
        {
            SmsChannelKey = channelKey,
            RequestUrl = Request.GetEncodedUrl(),
            RequestSignature = Request.Headers["X-Twilio-Signature"].FirstOrDefault(),
            Form = form.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault() ?? String.Empty, StringComparer.OrdinalIgnoreCase),
        };
    }
}