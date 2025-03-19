namespace Stride.Data.Models;

public class MockHabitRepository : IHabitRepository
{
    private readonly List<Habits> _habits;

    public MockHabitRepository()
    {
        _habits = new List<Habits>
        {
            new Habits
            {
                Id = 1,
                Title = "Morning Meditation",
                Description = "15 minutes mindfulness practice",
                Frequency = Frequency.Daily,
                ReminderTime = DateTime.Now.AddHours(8) 
            },
            new Habits
            {
                Id = 2,
                Title = "Weekly Exercise",
                Description = "1 hour gym session",
                Frequency = Frequency.Weekly,
                ReminderTime = DateTime.Now.AddHours(17) 
            },
            new Habits
            {
                Id = 3,
                Title = "Reading",
                Description = "Read for 30 minutes",
                Frequency = Frequency.Daily,
                ReminderTime = DateTime.Now.AddHours(21) 
            }
        };
    }

    public List<Habits> GetHabit()
    {
        return _habits;
    }

    public Habits GetHabitById(int id)
    {
        var habit = _habits.FirstOrDefault(h => h.Id == id);
        return habit ?? throw new KeyNotFoundException($"Habit with ID {id} not found");
    }

    public Habits CreateHabit(Habits habits)
    {
        habits.Id = _habits.Max(h => h.Id) + 1;
        _habits.Add(habits);
        return habits;
    }

    public Habits UpdateHabit(Habits habits)
    {
        var existingHabit = _habits.FirstOrDefault(h => h.Id == habits.Id);
        if (existingHabit != null)
        {
            existingHabit.Title = habits.Title;
            existingHabit.Description = habits.Description;
            existingHabit.Frequency = habits.Frequency;
            existingHabit.ReminderTime = habits.ReminderTime;
            return existingHabit;
        }
        throw new KeyNotFoundException($"Habit with ID {habits.Id} not found");
    }

    public Habits DeleteHabit(int id)
    {
        var habit = _habits.FirstOrDefault(h => h.Id == id);
        if (habit != null)
        {
            _habits.Remove(habit);
            return habit;
        }
        throw new KeyNotFoundException($"Habit with ID {id} not found");
    }
}