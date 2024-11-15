var bld = WebApplication.CreateBuilder(args);
bld.Services
   .AddHttpClient(
       "imdb",
       c =>
       {
           c.BaseAddress = new("https://m.imdb.com");
           c.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
           c.DefaultRequestHeaders.UserAgent.ParseAdd(
               "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36 Edg/130.0.0.0");
           c.Timeout = TimeSpan.FromSeconds(10);
       }).Services
   .AddHttpClient(
       "pushbullet",
       c =>
       {
           c.DefaultRequestHeaders.Add("Access-Token", bld.Configuration["Pushbullet:ApiKey"]);
           c.Timeout = TimeSpan.FromSeconds(10);
       }).Services
   .AddOutputCache()
   .AddFastEndpoints();

var app = bld.Build();
app.UseOutputCache()
   .UseFastEndpoints(
       c =>
       {
           c.Errors.UseProblemDetails();
           c.Endpoints.Configurator = ep => ep.Description(x => x.ProducesProblemDetails());
       });
app.Run();