using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models;

[Index(nameof(SubjectName))]
public class Lesson
{
    public int Id { get; set; }
    [ForeignKey(nameof(Subject))] public string SubjectName { get; set; } = "";

    public Subject? Subject { get; set; }
    public ICollection<Series> Series { get; set; } = [];
}