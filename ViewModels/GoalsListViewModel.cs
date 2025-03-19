using System;
using System.ComponentModel.DataAnnotations;
using Stride.Data.Models;

namespace Stride.ViewModels;
// for listing goals (strictly typed)
public class GoalListViewModel
{
    public IEnumerable<Goals> Goals { get; set; } = new List<Goals>();
}