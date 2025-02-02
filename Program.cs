

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
builder.Services.AddDbContext<ToDoDbContext>(); // שימוש במסד נתונים בזיכרון

var app = builder.Build();
app.UseCors("AllowAllOrigins");


app.MapGet("/items", (ToDoDbContext db) => db.Items.ToList());

app.MapPost("/items", (string name, ToDoDbContext db) => 
{
    Item item=new Item(){Name=name,IsComplete=false};
    db.Items.Add(item);
    db.SaveChanges();
    return Results.Created($"/items/{item.Id}", item);
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
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.MapGet("/",()=>"Authserver API is running");
app.Run();
