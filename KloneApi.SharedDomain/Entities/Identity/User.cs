using KloneApi.SharedDomain.BaseClasses;

namespace KloneApi.SharedDomain.Entities.Identity;

public class User : BaseEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string Pin { get; set; }
    public string Phone {get;set;}
    public string Email { get; set;}
    public bool IsAccountVerified {get;set;}
}