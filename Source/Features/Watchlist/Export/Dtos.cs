// ReSharper disable InconsistentNaming

#pragma warning disable CS8618

namespace ImdbWatchListManager.Watchlist.Export;

sealed class Edge
{
    public ListItem listItem { get; set; }
}

sealed class ListItem
{
    public string id { get; set; }
    public TitleText titleText { get; set; }
    public ReleaseYear? releaseYear { get; set; }
}

sealed class MainColumnData
{
    public PredefinedList predefinedList { get; set; }
}

sealed class PageProps
{
    public MainColumnData mainColumnData { get; set; }
}

sealed class PredefinedList
{
    public TitleListItemSearch titleListItemSearch { get; set; }
}

sealed class Props
{
    public PageProps pageProps { get; set; }
}

sealed class ReleaseYear
{
    public int year { get; set; }
}

sealed class JsonModel
{
    public Props props { get; set; }
}

sealed class TitleListItemSearch
{
    public List<Edge> edges { get; set; }
}

sealed class TitleText
{
    public string text { get; set; }
}

sealed class Movie
{
    public string Title { get; set; }
    public string Id { get; set; }
    public string Year { get; set; }
    public string Url => $"https://www.imdb.com/title/{Id}";
}