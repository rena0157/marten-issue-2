// See https://aka.ms/new-console-template for more information

using Marten;
using Vogen;
using Weasel.Core;

var connectionString =
    "User Id=postgres;Password=postgres;Server=localhost;Port=5432;Database=postgres;Include Error Detail=true;";

var store = DocumentStore.For(options =>
{
    options.Connection(connectionString);
    options.AutoCreateSchemaObjects = AutoCreate.All;

    options.RegisterValueType(typeof(EmailAddress));
});

await using var session = store.LightweightSession();

var customer = new Customer
{
    Email = EmailAddress.From("example@me.com")
};

session.Store(customer);

await session.SaveChangesAsync();

var loadedCustomer = await session.LoadAsync<Customer>(customer.Id);

if (loadedCustomer is null)
    return;

Console.WriteLine(loadedCustomer.Email);

// Throws exception currently
var queriedCustomer = await session.Query<Customer>()
    .FirstOrDefaultAsync(x => x.Email == customer.Email);

if (queriedCustomer is null)
    Console.WriteLine("Customer not found");

return;

[ValueObject<string>]
public partial record EmailAddress;

public class Customer
{
    public Guid Id { get; set; }
    
    public required EmailAddress Email { get; init; }
}