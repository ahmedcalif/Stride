using System.Collections.Generic;
using Stride.Data.DatabaseModels;

namespace Stride.Data.Models
{
    public interface IGoalRepository
    {
        IEnumerable<Goal> GetGoalsByUserId(int userId);
        IEnumerable<Goal> GetGoalsByUsername(string username);
        Goal? GetGoalById(int id);
        void Add(Goal goal);
        void Update(Goal goal);
        void Delete(int id);
    }
}