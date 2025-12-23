using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<WebBank.Domain.Models.BankContext>(options =>
    options.UseSqlite("Data Source=banca.db"));
// --------------------------------------
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseDefaultFiles(); // Cerca automaticamente index.html all'avvio
app.UseStaticFiles();  // Abilita la lettura della cartella wwwroot

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.Run();




