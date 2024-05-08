using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public class AddressEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;

    public ICollection<UserAddressEntity> UserAddresses { get; set; } = new List<UserAddressEntity>();
}
