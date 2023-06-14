namespace UpdateApi.Entities;

public class CustomerEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public string? Gender { get; set; }
    
    public DateTimeOffset? DeletedDate { get; set; }

    public bool IsDeleted() => DeletedDate != null;

    public void Delete(bool deleted)
    {
        if (!IsDeleted() && deleted)
            DeletedDate = DateTimeOffset.Now;
        else if (IsDeleted() && !deleted)
            DeletedDate = null;
    }

    public CustomerEntity Clone()
    {
        return (CustomerEntity)MemberwiseClone();
    }
}