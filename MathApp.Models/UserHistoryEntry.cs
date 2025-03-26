using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models;

public class UserHistoryEntry
{
    [Required]
    public string Id { get; set; } = "";
    [Required]
    public string ExerciseId { get; set; } = "";
    [Required]
    public string SeriesId { get; set; } = "";
    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;
    [Required]
    public int TimeSpent { get; set; } = 0;
    [Required]
    public bool Success { get; set; } = false;
}
