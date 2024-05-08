using Infrastructure.Entities;
using Infrastructure.Models;

namespace Infrastructure.Factories;

public class SignUpFactory
{
    public UserEntity PopulateUserEntity(SignUpRequest model)
    {
        return new UserEntity
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.Email
        };
    }
}
