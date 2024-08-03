using System.ComponentModel.DataAnnotations;

namespace KloneApi.SharedDomain.BaseClasses;

public class BaseEntity
{
    [Required]
    public DateTime? CreatedAt {get;set;} = DateTime.UtcNow;
    public DateTime? ModifiedAt {get;set;}

    [Required]
    public string? CreatedBy {get;set;}
    public string? ModifiedBy {get;set;}
}