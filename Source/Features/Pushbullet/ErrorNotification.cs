namespace ImdbWatchListManager.Features.Pushbullet;

sealed record ErrorNotification(string Title,
                                string Body) : ICommand;

sealed class ErrorNotificationHandler(IHttpClientFactory factory,
                                      ILogger<ErrorNotificationHandler> logger) : ICommandHandler<ErrorNotification>
{
    public async Task ExecuteAsync(ErrorNotification cmd, CancellationToken c)
    {
        var pushbullet = factory.CreateClient("pushbullet");

        if (pushbullet.DefaultRequestHeaders.TryGetValues("Access-Token", out var vals) && vals.Any(string.IsNullOrEmpty))
            return; //pb api key is not configured in appsettings.json

        var startTime = DateTime.Now;

        while (DateTime.Now.Subtract(startTime).TotalMinutes <= 60) //keep retrying up to an hour in case of error
        {
            try
            {
                var res = await pushbullet.PostAsJsonAsync(
                              "https://api.pushbullet.com/v2/pushes",
                              new
                              {
                                  type = "note",
                                  title = cmd.Title,
                                  body = cmd.Body
                              },
                              c);

                if (res.IsSuccessStatusCode)
                    return;

                logger.LogError(
                    "unable to send push notification! response status: [{statusCode}] error: [{error}]",
                    res.StatusCode.ToString(),
                    await res.Content.ReadAsStringAsync(c));
            }
            catch (Exception e)
            {
                logger.LogError("unable to send push notification! exception: [{error}]", e.Message);
            }
            await Task.Delay(60000); //wait 1 minute before trying again
        }
    }
}