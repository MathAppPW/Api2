using System.ComponentModel.DataAnnotations;

namespace Models;

public class Chapter
{
    [Key]
    public string Name { get; set; } = "";

    public ICollection<Lesson> Lessons { get; set; } = [];
}