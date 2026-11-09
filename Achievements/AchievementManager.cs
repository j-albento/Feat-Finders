using System.Collections.Generic;
using StardewModdingAPI;
using FeatFinders.Achievements;

/**
  This class updates an achievement's progress in response to game events
**/
namespace FeatFinders
{
  public class AchievementManager
  {
    // detect an event (player catches a fish) -> update the achievement's progress
    // Trigger the UI to update the tracker as well

    private readonly IMonitor Monitor;
    private readonly IModHelper Helper;

    private readonly string achievementsJSON = "Achievements/Achievements.json";

    // properties
    public List<Achievement> Achievements { get; private set; }

    public AchievementManager(IMonitor monitor, IModHelper helper)
    {
      Monitor = monitor;
      Helper = helper;

      LoadAchievements();
      ValidateAchievements();
    }



    // Load the Achievements json file and create a list of Achievement objects
    private void LoadAchievements()
    {
      Achievements = Helper.Data.ReadJsonFile<List<Achievement>>(achievementsJSON);
      if (Achievements == null)
      {
        Monitor.Log("Could not find or load achievements.json.", LogLevel.Error);
        Achievements = new List<Achievement>();
      }
      else
      {
        Monitor.Log($"Validating {Achievements.Count} achievements...", LogLevel.Info);
      }
    }

    private void ValidateAchievements()
    {
      if (Achievements == null)
      {
        Monitor.Log("Achievement list is null. Cannot validate achievements.", LogLevel.Error);
        return;
      }

      // Keep track of invalid achievements.
      int invalidCount = 0;

      for (int i = Achievements.Count - 1; i >= 0; i--)
      {
        Achievement achievement = Achievements[i];

        // Check for missing required information.
        if (achievement == null ||
            string.IsNullOrWhiteSpace(achievement.Id) ||
            string.IsNullOrWhiteSpace(achievement.Name) ||
            string.IsNullOrWhiteSpace(achievement.Description) ||
            string.IsNullOrWhiteSpace(achievement.Type))
        {
          Monitor.Log(
              $"Achievement at index {i} is missing required information and will be excluded.",
              LogLevel.Error
          );

          Achievements.RemoveAt(i);
          invalidCount++;
        }
      }

      Monitor.Log(
          $"Achievement validation complete. {Achievements.Count} valid achievements, {invalidCount} invalid achievements.",
          LogLevel.Info
      );
    }



    // increments an achievement's current value (progress)
    public void UpdateProgress(Achievement achievement)
    {
      achievement.Increment();
    }


    // marks an achivement as complete only if isComplete is true
    public static void MarkComplete(Achievement achievement)
    {
      achievement.Completed = true;
    }


    public void UpdateUniqueFishProgress(int uniqueFishCount)
    {
      const int totalFishRequired = 72;

      foreach (Achievement achievement in Achievements)
      {
        if (achievement.Completed)
          continue;

        if (achievement.Type != "collection" ||
            achievement.CollectionType != "fish")
          continue;

        // handles MasterAngler separately
        if (achievement.Id == "MasterAngler")
        {
          achievement.CurrentValue = uniqueFishCount;

          if (uniqueFishCount >= totalFishRequired)
          {
            MarkComplete(achievement);

            Monitor.Log("Unlocked: " + achievement.Name, LogLevel.Info);
          }
          continue;
        }

        // handles Fisherman and OlMariner
        if (!achievement.TargetValue.HasValue) continue;

        if (achievement.CheckCompletion())
        {
          Monitor.Log("Unlocked: " + achievement.Name, LogLevel.Info);
        }

      }
    }
    public void UpdateTotalFishProgress(uint totalFishCaught)
    {
      foreach (Achievement achievement in Achievements)
      {
        if (achievement.Completed)
          continue;

        if (achievement.Type != "counter" ||
            achievement.CounterType != "fishCaught")
          continue;

        achievement.CurrentValue = totalFishCaught;

        if (achievement.CheckCompletion())
        {
          Monitor.Log(
              $"Unlocked: {achievement.Name}!",
              LogLevel.Info
          );
        }
      }
    }
  }
}