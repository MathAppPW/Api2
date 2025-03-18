using Microsoft.EntityFrameworkCore;

namespace Models;

public class Exercise
{
    public int Id { get; set; }
    public string Contents { get; set; } = "";

    public ICollection<Series> Series { get; set; } = [];
}