namespace Stride.Data.Models;

public class MockGoalRepository : IGoalRepository
{
    private readonly List<Goals> _goals;

    public MockGoalRepository()
    {
        _goals = new List<Goals>
        {
            new Goals
            {
                Id = 1,
                Title = "Complete Project A",
                Description = "Finish all main features of Project A",
                TargetDate = DateTime.Now.AddDays(30),
                Priority = Priority.High,
                Category = "School",
                IsCompleted = false
            },
            new Goals
            {
                Id = 2,
                Title = "Learn New Technology",
                Description = "Master ASP.NET Core",
                TargetDate = DateTime.Now.AddDays(60),
                Priority = Priority.Medium,
                Category = "Coding",
                IsCompleted = false
            },
            new Goals
            {
                Id = 3,
                Title = "Write Documentation",
                Description = "Document Project A features",
                TargetDate = DateTime.Now.AddDays(15),
                Priority = Priority.Low,
                Category = "Documentation",
                IsCompleted = true
            }
        };
    }

    public IEnumerable<Goals> GetAllGoals()
    {
        return _goals;
    }

    public Goals? GetGoalById(int id)
    {
        return _goals.FirstOrDefault(g => g.Id == id);
    }

    public Goals Add(Goals goal)
    {
        goal.Id = _goals.Max(g => g.Id) + 1;
        _goals.Add(goal);
        return goal;
    }

    public Goals Update(Goals goal)
    {
        var existingGoal = _goals.FirstOrDefault(g => g.Id == goal.Id);
        if (existingGoal != null)
        {
            existingGoal.Title = goal.Title;
            existingGoal.Description = goal.Description;
            existingGoal.TargetDate = goal.TargetDate;
            existingGoal.Priority = goal.Priority;
            existingGoal.Category = goal.Category;
            existingGoal.IsCompleted = goal.IsCompleted;
        }
        return existingGoal ?? throw new KeyNotFoundException($"Goal with ID {goal.Id} not found");
    }

    public Goals Delete(int id)
    {
        var goal = _goals.FirstOrDefault(g => g.Id == id);
        if (goal != null)
        {
            _goals.Remove(goal);
            return goal;
        }
        throw new KeyNotFoundException($"Goal with ID {id} not found");
    }

    public IEnumerable<Goals> GetIncompleteGoals()
    {
        return _goals.Where(g => !g.IsCompleted);
    }

    public IEnumerable<Goals> GetGoalsByUsername(string username)
    {
        return _goals.Where(g => g.Username == username);
    }
}