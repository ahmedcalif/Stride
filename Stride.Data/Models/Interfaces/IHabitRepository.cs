namespace Stride.Data.Models;


public interface IHabitRepository {

  List<Habits> GetHabit();

  Habits GetHabitById(int id);

  Habits CreateHabit(Habits habits);

  Habits UpdateHabit(Habits habits);

  Habits DeleteHabit(int id);

}