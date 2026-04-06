using MenuHelper.Domain.DomainEvents;

namespace MenuHelper.Domain.AggregatesModel.SupplierAggregate;

public partial record SupplierId : IGuidStronglyTypedId;

public class Supplier : Entity<SupplierId>, IAggregateRoot
{
    protected Supplier() { }

    public Supplier(string name, string? phone, string? remark)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new KnownException("供应商名称不能为空");
        Name = name;
        Phone = phone;
        Remark = remark;
        this.AddDomainEvent(new SupplierCreatedDomainEvent(this));
    }

    public string Name { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Remark { get; private set; }
    public Deleted Deleted { get; private set; } = new();
    public RowVersion RowVersion { get; private set; } = new(0);

    public void Update(string name, string? phone, string? remark)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new KnownException("供应商名称不能为空");
        Name = name;
        Phone = phone;
        Remark = remark;
        this.AddDomainEvent(new SupplierUpdatedDomainEvent(this));
    }

    public void Delete()
    {
        if (Deleted)
            throw new KnownException("供应商已删除");
        Deleted = new Deleted(true);
        this.AddDomainEvent(new SupplierDeletedDomainEvent(this));
    }
}
