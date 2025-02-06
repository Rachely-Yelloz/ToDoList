
using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

var app = builder.Build();
app.UseCors("AllowAllOrigins");

app.MapGet("/items", async (ToDoDbContext db) => { return await db.Items.ToListAsync(); });

app.MapPost("/items/", (Item item, ToDoDbContext db) => 
{
    db.Items.Add(item);
    db.SaveChanges(); 
    return Results.Created($"/{item.Id}", item); 
});

app.MapPut("/items/{id}", (int id, Item updatedItem, ToDoDbContext db) => 
{
    var item = db.Items.Find(id);
    if (item is null) return Results.NotFound();
    
    // עדכון רק את הסטטוס אם הוא נשלח
    if (updatedItem.IsComplete.HasValue)
    {
        item.IsComplete = updatedItem.IsComplete;
    }
    if (!string.IsNullOrEmpty(updatedItem.Name))
    {
        item.Name = updatedItem.Name;
    }
    
    db.SaveChanges();
    return Results.NoContent();
});

app.MapDelete("/items/{id}", (int id, ToDoDbContext db) => 
{
    var item = db.Items.Find(id);
    if (item is null) return Results.NotFound();
    
    db.Items.Remove(item);
    db.SaveChanges();
    return Results.NoContent();
});

app.UseSwagger();  
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
    c.RoutePrefix = string.Empty;
});

app.MapGet("/",()=>"Authserver API is running");
app.Run();
