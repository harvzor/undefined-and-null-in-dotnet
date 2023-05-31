using UpdateApi.Customer.Dtos;
using UpdateApi.Customer.Entities;

namespace UpdateApi.Customer.Mappers;

public static class CustomerMapper
{
    public static CustomerDto Map(this CustomerEntity customerEntity) =>
        new CustomerDto
        {
            Id = customerEntity.Id,
            Name = customerEntity.Name,
        };

    public static IEnumerable<CustomerDto> Map(this IEnumerable<CustomerEntity> customerEntities) =>
        customerEntities.Select(x => x.Map());
}
