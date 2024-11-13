﻿using System.Text;
using System.Text.Json;
using HtmlAgilityPack;

namespace ImdbWatchListManager.Watchlist.Export;

sealed class Endpoint(IHttpClientFactory factory) : Ep.NoReq.NoRes
{
    public override void Configure()
    {
        Get("watchlist/export");
        AllowAnonymous();
        Description(x => x.Produces<object>(200, "text/csv"));
        Options(x => x.CacheOutput(p => p.Expire(TimeSpan.FromHours(1))));
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var userId = Config["Imdb:UserId"];
        if (!userId?.StartsWith("ur") is true)
            ThrowError("imdb user id is not configured in app settings!");

        var html = new HtmlDocument();
        var imdb = factory.CreateClient("imdb");

        try
        {
            html.Load(await imdb.GetStreamAsync($"user/{userId}/watchlist/?view=compact", c));
        }
        catch (Exception e)
        {
            ThrowError($"unable to download watchlist. details: [{e.Message}]");
        }

        var jsonString = html.DocumentNode.SelectSingleNode("//script[@id=\"__NEXT_DATA__\"]").InnerText;
        if (string.IsNullOrEmpty(jsonString))
            ThrowError("unable to parse json data from page!");

        var movies = JsonSerializer.Deserialize<JsonModel>(jsonString)?
                                   .props.pageProps.mainColumnData.predefinedList.titleListItemSearch.edges
                                   .Select(
                                       e => new Movie
                                       {
                                           Id = e.listItem.id,
                                           Title = e.listItem.titleText.text,
                                           Year = e.listItem.releaseYear?.year.ToString() ?? string.Empty
                                       });

        if (movies?.Any() is not true)
            ThrowError("unable to parse movies from the watchlist!");

        var sb = new StringBuilder();

        foreach (var m in movies)
        {
            sb.Append(Sanitize(m.Title!)).Append(',')
              .Append(m.Year).Append(',')
              .Append(m.Id).Append(',')
              .Append(m.Url).AppendLine();
        }

        await SendStringAsync(sb.ToString(), contentType: "text/csv", cancellation: c);
    }

    static string Sanitize(string val)
    {
        if (val.Contains('"'))
            val = val.Replace("\"", "\"\"");

        if (val.Contains(',') || val.Contains('\n') || val.Contains('\r'))
            val = $"\"{val}\"";

        return val;
    }
}