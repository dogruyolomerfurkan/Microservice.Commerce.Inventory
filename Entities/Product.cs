using Commerce.Core;

namespace Commerce.Inventory.Service.Entities;

public class Product : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}