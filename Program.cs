
using Microsoft.AspNetCore.Mvc;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// Added as service
builder.Services.AddSingleton<ToDoDbContext>();

var app = builder.Build();
app.MapGet("/items", (ToDoDbContext db) => 
{
    return db.Items.ToList();
});
app.Run();


