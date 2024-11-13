var bld = WebApplication.CreateBuilder(args);
bld.Services
   .AddHttpClient("imdb", c => c.BaseAddress = new("https://m.imdb.com")).Services
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