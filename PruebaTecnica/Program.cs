using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Data;
using PruebaTecnica.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PDbContext>(options =>
    options.UseSqlite("Data Source=data.db"));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

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
