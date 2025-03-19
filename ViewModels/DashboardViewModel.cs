using System.Collections.Generic;
using Stride.Data.Models;

namespace Stride.ViewModels
{
    public class DashboardViewModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IEnumerable<Goals> UserGoals { get; set; }
    
        public int CompletedGoalsCount => UserGoals?.Count(g => g.IsCompleted) ?? 0;
        public int PendingGoalsCount => UserGoals?.Count(g => !g.IsCompleted) ?? 0;
    }
}