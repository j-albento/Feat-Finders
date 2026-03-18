/**
  This class updates an achivement's progress in response to game events


**/



public class AchievementManager
{
  // detect an event (player catches a fish) -> update the achievement's progress
  // Trigger the UI to update the tracker as well

  // properties
  List<Achievement> achievements;

  


  // increments an achievement's current value (progress)
  public void UpdateProgress(Achievement achievement)
  {
    achievement.Increment();
  }


  // marks an achivement as complete only if isComplete is true
  private void MarkComplete(Achievement achievement, bool isComplete)
  {
    if (isComplete)
    {
      achievement.setCompleted(true);
    }
  }
}