using Temporalio.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// TemporalClient ‚ğ“¯ŠúƒuƒƒbƒN‚Å¶¬‚µ‚Ä Singleton “o˜^
builder.Services.AddSingleton<TemporalClient>(_ =>
{
    var target = Environment.GetEnvironmentVariable("TEMPORAL_TARGET") ?? "localhost:7233";
    return TemporalClient.ConnectAsync(new(target)).GetAwaiter().GetResult();
});

// Worker ‚ğ HostedService ‚Æ‚µ‚Äí’“
builder.Services.AddHostedService<TemporalWorkerHostedService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
