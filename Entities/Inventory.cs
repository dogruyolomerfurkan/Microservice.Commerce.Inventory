using Commerce.Core;

namespace Commerce.Inventory.Service.Entities;

public class Inventory : IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTimeOffset AcquiredDate { get; set; }
}