using Microsoft.EntityFrameworkCore;
using PizzaStore.SQLite.API.Data;
using PizzaStore.SQLite.API.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "PizzaStore SQLite API",
        Description = "PizzaStore API with SQLite database",
        Version = "v1"
    });
});



builder.Services.AddDbContext<PizzaDb>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore SQLite API V1");
    });
}


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PizzaDb>();
    context.Database.EnsureCreated();
    
   
    if (!context.Pizzas.Any())
    {
        context.Pizzas.AddRange(
            new Pizza { Id = 1, Name = "Margherita", Description = "Classic pizza with tomatoes and mozzarella" },
            new Pizza { Id = 2, Name = "Pepperoni", Description = "Pizza with spicy pepperoni and cheese" },
            new Pizza { Id = 3, Name = "Hawaiian", Description = "Pizza with ham and pineapple" }
        );
        context.SaveChanges();
    }
}

app.UseHttpsRedirection();

// API Endpoints
app.MapGet("/", () => "Welcome to PizzaStore SQLite API!");

// Get all pizzas
app.MapGet("/pizzas", async (PizzaDb db) =>
    await db.Pizzas.ToListAsync());

// Get pizza by id
app.MapGet("/pizzas/{id}", async (int id, PizzaDb db) =>
    await db.Pizzas.FindAsync(id)
        is Pizza pizza
            ? Results.Ok(pizza)
            : Results.NotFound());

// Create a new pizza
app.MapPost("/pizzas", async (Pizza pizza, PizzaDb db) =>
{
    db.Pizzas.Add(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizzas/{pizza.Id}", pizza);
});

// Update a pizza
app.MapPut("/pizzas/{id}", async (int id, Pizza updatedPizza, PizzaDb db) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    
    if (pizza is null) return Results.NotFound();
    
    pizza.Name = updatedPizza.Name;
    pizza.Description = updatedPizza.Description;
    
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Delete a pizza
app.MapDelete("/pizzas/{id}", async (int id, PizzaDb db) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    
    if (pizza is null) return Results.NotFound();
    
    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    
    return Results.Ok();
});

app.Run();
