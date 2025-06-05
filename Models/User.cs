namespace UserAggregator.Models;

public class User
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SourceId { get; set; } = string.Empty;

    public User() { }

    public User(string firstName, string lastName, string email, string sourceId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        SourceId = sourceId;
    }

    public override string ToString() =>
        $"{FirstName} {LastName} <{Email}> (ID: {SourceId})";
}
