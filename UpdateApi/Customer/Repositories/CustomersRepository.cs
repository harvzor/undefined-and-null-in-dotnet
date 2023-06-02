using UpdateApi.Customer.Dtos.Input;
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
        
        return customer;
    }
    
    public CustomerEntity? Update(int id, BrokenCustomerPutDto brokenCustomerPutDto)
    {
        var customer = Customers
            .FirstOrDefault(x => x.Id == id);

        if (customer == null)
            return null;

        customer.Name = brokenCustomerPutDto.Name;
        customer.Gender = brokenCustomerPutDto.Gender;

        return customer.Clone();
    }
    
    public CustomerEntity? Update(int id, DotNextOptionalCustomerPutDto dotNextOptionalCustomerPutDto)
    {
        var customer = Customers
            .FirstOrDefault(x => x.Id == id);

        if (customer == null)
            return null;

        customer.Name = dotNextOptionalCustomerPutDto.Name;
        
        if (dotNextOptionalCustomerPutDto.Gender.HasValue)
            customer.Gender = dotNextOptionalCustomerPutDto.Gender.OrDefault();

        return customer.Clone();
    }
}
