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
    
    public CustomerEntity? Update(int id, BrokenCustomerPutDto brokenCustomerPutDto)
    {
        var customer = Customers
            .FirstOrDefault(x => x.Id == id);

        if (customer == null)
            return null;

        customer.Name = brokenCustomerPutDto.Name;
        customer.Gender = brokenCustomerPutDto.Gender;

        return customer;
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

        return customer;
    }
}
