using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Stride.Data.DatabaseModels;
using Stride.Data.Models;
using Stride.Data;
using Stride.Data.Data;

namespace Stride.Data.Models.SQLRepository
{
    public class SQLGoalRepository : IGoalRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SQLGoalRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Goal> GetGoalsByUserId(int userId)
        {
             return _dbContext.Goals
        .Include(g => g.Category)   
        .Include(g => g.Priority)      
        .Where(g => g.user_id == userId)
        .ToList();
        }

        public IEnumerable<Goal> GetGoalsByUsername(string username)
{
    // Find the user first
    var user = _dbContext.Users.FirstOrDefault(u => u.username == username);
    if (user == null)
    {
        return new List<Goal>();
    }
    
    // Then get their goals
    return _dbContext.Goals
        .Include(g => g.Category)
        .Include(g => g.Priority)
        .Where(g => g.user_id == user.user_id)
        .ToList();
}

        public Goal GetGoalById(int id)
        {
            return _dbContext.Goals
                .Include(g => g.Category)
                .Include(g => g.Priority)
                .FirstOrDefault(g => g.goal_id == id);
        }

        public void Add(Goal goal)
        {
            _dbContext.Goals.Add(goal);
            _dbContext.SaveChanges();
        }

        public void Update(Goal goal)
        {
            _dbContext.Goals.Update(goal);
            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var goal = _dbContext.Goals.Find(id);
            if (goal != null)
            {
                _dbContext.Goals.Remove(goal);
                _dbContext.SaveChanges();
            }
        }
    }
}