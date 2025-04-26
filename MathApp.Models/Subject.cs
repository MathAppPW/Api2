using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models;

[Index(nameof(ChapterName))]
public class Subject
{
    [Key] public string Name { get; set; } = "";
    [ForeignKey(nameof(Chapter))] public string ChapterName { get; set; } = "";
    
    public Chapter? Chapter { get; set; }
    public Theory? Theory { get; set; }
    public ICollection<Lesson> Lessons { get; set; } = [];
}