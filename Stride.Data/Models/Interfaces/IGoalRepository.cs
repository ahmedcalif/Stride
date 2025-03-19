namespace Stride.Data.Models;


public interface IGoalRepository
{
    IEnumerable<Goals> GetAllGoals();
    Goals? GetGoalById(int id);
    Goals Add(Goals goal);
    Goals Update(Goals goal);
    Goals Delete(int id);
    IEnumerable<Goals> GetIncompleteGoals();


    IEnumerable<Goals> GetGoalsByUsername(string username);
}
