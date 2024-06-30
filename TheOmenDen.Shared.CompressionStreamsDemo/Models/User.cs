namespace TheOmenDen.Shared.CompressionStreamsDemo.Models;


public enum Gender
{
    Male,
    Female,
    NonBinary,
    Agender,
    PrefersNotToSay
}

public class User(int userId, string ssn)
{
    public int Id { get; set; } = userId;
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string SomethingUnique { get; set; }
    public Guid SomeGuid { get; set; }
    public string Avatar { get; set; }
    public Guid CartId { get; set; }
    public string SSN { get; set; } = ssn;
    public Gender Gender { get; set; }
    public List<Order> Orders { get; set; }
}