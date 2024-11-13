# ImdbWatchlistManager
a web app to export your imdb watchlist to a csv file.

## how to use:

1. grab the correct binaries for your OS from the `Releases` section.
2. extract the zip to a folder.
3. edit the `appsettings.json` file and enter your imdb user id which is in the form of `ur1234567890`.
4. launch the web app by executing the main binary `ImdbWatchlistManager`
5. make a `GET` request to the url: `http://localhost:5000/watchlist/export`

the app will scrape your imdb watchlist page (which should be make public) and convert the data to csv format. 
the csv output is cached in memory for 1 hour. i.e. imdb is scraped no more than once per hour, even if the endpoint is hit repeatedly.

## todo:

- remove a previously watchlisted movie from the watchlist by supplying it's imdb id.