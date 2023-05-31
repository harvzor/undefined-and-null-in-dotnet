using UpdateApi.Customer.Entities;

namespace UpdateApi.Customer.Repositories;

public class CustomersRepository
{
    private static readonly HashSet<CustomerEntity> Customers = new();

    public CustomersRepository()
    {
        if (!Customers.Any())
        {
            // Seed
            Add(new CustomerEntity()
            {
                Id = 1,
                Name = "A",
            });
            
            Add(new CustomerEntity()
            {
                Id = 2,
                Name = "B",
            });
            
            Add(new CustomerEntity()
            {
                Id = 3,
                Name = "C",
            });
        }
    }
    
    public void Add(CustomerEntity customer)
    {
        Customers.Add(customer);
    }
    
    public CustomerEntity? Find(int id)
    {
        return Customers
            .FirstOrDefault(x => x.Id == id);
    }

    public CustomerEntity[] GetAll()
    {
        return Customers
            .ToArray();
    }
}
