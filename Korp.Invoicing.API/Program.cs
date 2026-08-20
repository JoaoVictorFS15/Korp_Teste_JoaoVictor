using Microsoft.EntityFrameworkCore;
using Korp.Invoicing.API.Data;
using Korp.Invoicing.API.Services;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<InvoicingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurando o tratamento de falhas com Polly (Resiliência)
builder.Services.AddHttpClient<IStockService, StockService>(client =>
{
    // Porta padrão onde a API de Estoque vai rodar localmente (verifique o launchSettings.json, assumiremos 5001/5000 mas pode variar).
    // Para fins do teste, injetamos direto ou via config. Assumiremos que a API de estoque roda na porta 5020.
    client.BaseAddress = new Uri("https://localhost:7229");
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

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

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
