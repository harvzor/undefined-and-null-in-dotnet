using UpdateApi.Entities;

namespace UpdateApi.Repositories;

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
                DeletedDate = null,
            });
            
            Add(new CustomerEntity()
            {
                Id = 2,
                Name = "B",
                DeletedDate = null,
            });
            
            Add(new CustomerEntity()
            {
                Id = 3,
                Name = "C",
                DeletedDate = null,
            });
        }
    }
    
    public void Add(CustomerEntity customer)
    {
        Customers.Add(customer.Clone());
    }
    
    public CustomerEntity? Find(int id)
    {
        var customer = Customers
            .FirstOrDefault(x => x.Id == id);

        if (customer == null)
            return customer;

        return customer.Clone();
    }

    public CustomerEntity[] GetAll()
    {
        return Customers
            .Select(x => x.Clone())
            .ToArray();
    }
    
    public CustomerEntity? Update(CustomerEntity updateCustomer)
    {
        var customer = Customers
            .FirstOrDefault(x => x.Id == updateCustomer.Id);
        
        if (customer == null)
            return null;

        customer.Name = updateCustomer.Name;
        customer.Gender = updateCustomer.Gender;
        customer.DeletedDate = updateCustomer.DeletedDate;
        
        return customer;
    }
}
