var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory list to simulate data storage
var items = new List<string> { "Item1", "Item2", "Item3" };

// GET - Retrieve all items
app.MapGet("/items", () =>
{
    return Results.Ok(items);
});

// GET - Retrieve an item by index
app.MapGet("/items/{id:int}", (int id) =>
{
    if (id < 0 || id >= items.Count)
        return Results.NotFound("Item not found.");

    return Results.Ok(items[id]);
});

app.MapPost("/items", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var newItem = await reader.ReadToEndAsync();

    if (string.IsNullOrWhiteSpace(newItem))
        return Results.BadRequest("Item cannot be empty.");

    items.Add(newItem);
    return Results.Created($"/items/{items.Count - 1}", newItem);
});


// PUT - Update an existing item by index
app.MapPut("/items/{id:int}", async (HttpRequest request, int id) =>
{
    using var reader = new StreamReader(request.Body);
    var updatedItem = await reader.ReadToEndAsync();

    if (id < 0 || id >= items.Count)
        return Results.NotFound("Item not found.");

    if (string.IsNullOrWhiteSpace(updatedItem))
        return Results.BadRequest("Updated item cannot be empty.");

    items[id] = updatedItem;
    return Results.Ok(items[id]);
});


// DELETE - Delete an item by index
app.MapDelete("/items/{id:int}", (int id) =>
{
    if (id < 0 || id >= items.Count)
        return Results.NotFound("Item not found.");

    items.RemoveAt(id);
    return Results.Ok("Item deleted.");
});

app.Run();
