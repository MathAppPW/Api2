using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models;

public class LeaderboardEntry
{
    public int Id { get; set; }
    public UserProfile User { get; set; } = new();
    public int Score { get; set; }
}
