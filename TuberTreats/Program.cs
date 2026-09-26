using TuberTreats.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

List<Customer> customers = new()
{
    new Customer
    {
        Id = 1,
        Name = "Alice",
        Address = "100 Main Street"
    },
     new Customer
    {
        Id = 2,
        Name = "Bob",
        Address = "200 Oak Avenue"
    },
     new Customer
    {
        Id = 3,
        Name = "Carla",
        Address = "300 Pine Road"
    },
     new Customer
    {
        Id = 4,
        Name = "David",
        Address = "400 Elm Street"
    },
     new Customer
    {
        Id = 5,
        Name = "Erin",
        Address = "500 Maple Drive"
    }
};
List<TuberDriver> tuberDrivers = new()
{
    new TuberDriver
    {
       Id = 1,
       Name = "Driver One"
    },
     new TuberDriver
    {
       Id = 2,
       Name = "Driver Two"
    }, new TuberDriver
    {
       Id = 3,
       Name = "Driver Three"
    }
};
List<Topping> toppings = new()
{
    new Topping
    {
        Id = 1,
        Name = "Cheese"
    },
     new Topping
    {
        Id = 2,
        Name = "Bacon"
    },
        new Topping
    {
        Id = 3,
        Name = "Sour Cream"
    },
        new Topping
    {
        Id = 4,
        Name = "Chives"
    },
        new Topping
    {
        Id = 5,
        Name = "Butter"
    }
};
List<TuberOrder> tuberOrders = new()
{
    new TuberOrder
    {
        Id = 1,
        OrderPlacedOnDate = DateTime.Now.AddHours(-3),
        CustomerId = 1,
        TuberDriverId = 1,
        DeliveredOnDate = DateTime.Now.AddHours(-2)
    },
        new TuberOrder
    {
        Id = 2,
        OrderPlacedOnDate = DateTime.Now.AddHours(-2),
        CustomerId = 2,
        TuberDriverId = 2,
        DeliveredOnDate = null

    },
        new TuberOrder
    {
        Id = 3,
        OrderPlacedOnDate = DateTime.Now.AddHours(-1),
        CustomerId = 5,
        TuberDriverId = 1,
        DeliveredOnDate = null

    }
};
List<TuberTopping> tuberToppings = new()
{
    new TuberTopping
    {
        Id = 1,
        TuberOrderId = 1,
        ToppingId = 1
    },
       new TuberTopping
    {
        Id = 2,
        TuberOrderId = 1,
        ToppingId = 2
    },
       new TuberTopping
    {
        Id = 3,
        TuberOrderId = 2,
        ToppingId = 3
    },

};

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//add endpoints here
app.MapGet("/toppings", () =>
{
    return toppings;
});

app.MapGet("/toppings/{id}", (int id) =>
{
    Topping topping = toppings.FirstOrDefault(t => t.Id == id);
    if (topping == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(topping);
});

app.MapGet("/tuberorders", () =>
{
    return tuberOrders;
});

app.MapGet("/tuberorders/{id}", (int id) =>
{
    TuberOrder order = tuberOrders.FirstOrDefault(o => o.Id == id);

    if (order == null)
    {
        return Results.NotFound();
    }

    order.Toppings = tuberToppings
    .Where(tt => tt.TuberOrderId == order.Id)
    .Select(tt => toppings.First(t => t.Id == tt.ToppingId))
    .ToList();

    return Results.Ok(order);
});

app.MapPost("/tuberorders", (TuberOrder order) =>
{
    order.Id = tuberOrders.Max(o => o.Id) + 1;
    order.OrderPlacedOnDate = DateTime.Now;
    order.Toppings = new List<Topping>();

    tuberOrders.Add(order);

    return Results.Ok(order);
});

app.MapPut("/tuberorders/{id}", (int id, TuberOrder updatedOrder) =>
{
    TuberOrder order = tuberOrders.FirstOrDefault(o => o.Id == id);

    if (order == null)
    {
        return Results.NotFound();
    }

    order.TuberDriverId = updatedOrder.TuberDriverId;
    return Results.NoContent();
});

app.MapPost("/tuberorders/{id}/complete", (int id) =>
{
    TuberOrder order = tuberOrders.FirstOrDefault(o => o.Id == id);

    if (order == null)
    {
        return Results.NotFound();
    }

    order.DeliveredOnDate = DateTime.Now;

    return Results.NoContent();

});

app.MapGet("/tuberdrivers", () =>
{
    return tuberDrivers;
});

app.MapGet("/tubertoppings", () =>
{
    return tuberToppings;
});

app.MapPost("/tubertoppings", (TuberTopping tuberTopping) =>
{
    tuberTopping.Id = tuberToppings.Max(tt => tt.Id) + 1;

    tuberToppings.Add(tuberTopping);
    return Results.Ok(tuberTopping);
});

app.MapDelete("/tubertoppings/{id}", (int id) =>
{
    TuberTopping tuberTopping = tuberToppings.FirstOrDefault(tt => tt.Id == id);

    if (tuberTopping == null)
    {
        return Results.NotFound();
    }

    tuberToppings.Remove(tuberTopping);

    return Results.NoContent();
});

app.MapGet("/customers", () =>
{
    return customers;
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);

    if (customer == null)
    {
        return Results.NotFound();
    }

    customer.TuberOrders = tuberOrders
    .Where(o => o.CustomerId == customer.Id)
    .ToList();

    return Results.Ok(customer);
});

app.MapPost("/customers", (Customer customer) =>
{
    customer.Id = customers.Max(c => c.Id) + 1;
    customer.TuberOrders = new List<TuberOrder>();
    customers.Add(customer);
    return Results.Ok(customer);
});

app.MapDelete("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);

    if (customer == null)
    {
        return Results.NotFound();
    }
    customers.Remove(customer);
    return Results.NoContent();
});

app.MapGet("/tuberdrivers/{id}", (int id) =>


{
    TuberDriver driver = tuberDrivers.FirstOrDefault(d => d.Id == id);

    if (driver == null)
    {
        return Results.NotFound();
    }

    driver.TuberDeliveries = tuberOrders
    .Where(o => o.TuberDriverId == driver.Id)
    .ToList();

    return Results.Ok(driver);

});



app.Run();
//don't touch or move this!
public partial class Program { }