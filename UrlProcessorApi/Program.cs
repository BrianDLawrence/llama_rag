var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<UrlLoaderService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/process-urls", async (HttpContext context, UrlLoaderService urlLoaderService) =>
{
    var urls = await context.Request.ReadFromJsonAsync<List<string>>();
    if (urls == null || urls.Count == 0)
    {
        return Results.BadRequest("No URLs provided.");
    }

    var urlData = await urlLoaderService.ProcessUrls(urls);
    return Results.Ok(urlData);
});

app.Run();

public partial class Program { }
