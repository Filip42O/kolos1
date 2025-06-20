using KolokwiumTemplate.Service;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja kontrolerów
builder.Services.AddControllers();

// Rejestracja serwisu
builder.Services.AddScoped<IDbService, DbService>();

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseAuthorization();

// Mapowanie kontrolerów
app.MapControllers();

app.Run();