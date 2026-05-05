var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
var notes = new List<Note>();
app.MapGet("/health", () => new { status = "ok", time = DateTime.Now });

app.MapGet("/version", (IConfiguration conf) => new {
    name = conf["App:Name"],
    version = conf["App:Version"]
});
app.MapGet("/api/notes", () => notes);

app.MapPost("/api/notes", (Note note) => {
    if (string.IsNullOrEmpty(note.Title)) return Results.BadRequest("Title is required");
    notes.Add(note);
    return Results.Created($"/api/notes/{note.Id}", note);
});

app.MapGet("/api/notes/{id}", (int id) =>
    notes.FirstOrDefault(n => n.Id == id) is Note n ? Results.Ok(n) : Results.NotFound());

app.MapDelete("/api/notes/{id}", (int id) => {
    notes.RemoveAll(n => n.Id == id);
    return Results.NoContent();
});

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/db/ping", (IConfiguration conf) => {
    var connectionString = conf.GetConnectionString("Mssql");
    return Results.Problem($"Попытка подключения к БД: {connectionString}. Ошибка: База данных еще не развернута.");
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


public class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
