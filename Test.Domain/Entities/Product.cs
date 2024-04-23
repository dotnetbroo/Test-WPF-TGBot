using Test.Domain.Commons;

namespace Test.Domain.Entities;

public class Product : Auditable
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Video { get; set; }
    public long SortNumber { get; set; }
}
