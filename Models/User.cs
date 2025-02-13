using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Username { get; set; }

    public string ApiKey { get; set; } = GenerateApiKey();

    private static string GenerateApiKey()
    {
        return Guid.NewGuid().ToString(); // Generates a unique API Key
    }
}
