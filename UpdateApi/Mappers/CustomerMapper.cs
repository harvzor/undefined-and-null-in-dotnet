using UpdateApi.Dtos.Output;
using UpdateApi.Entities;

namespace UpdateApi.Mappers;

public static class CustomerMapper
{
    public static CustomerDto Map(this CustomerEntity customerEntity) =>
        new()
        {
            Id = customerEntity.Id,
            Name = customerEntity.Name,
            Gender = customerEntity.Gender,
            DeletedDate = customerEntity.DeletedDate,
        };

    public static IEnumerable<CustomerDto> Map(this IEnumerable<CustomerEntity> customerEntities) =>
        customerEntities.Select(x => x.Map());
}
