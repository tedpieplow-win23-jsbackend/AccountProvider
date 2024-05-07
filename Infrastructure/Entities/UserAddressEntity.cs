using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class UserAddressEntity
{
    [Key]
    [Column(Order = 1)]
    public string UserId { get; set; } = null!;
    [Key]
    [Column(Order = 2)]
    public string AddressId { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
    public AddressEntity Address { get; set; } = null!;
}
