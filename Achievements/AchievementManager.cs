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

    public void LoadProgress()
    {
      // Reset progress first, so another save file cannot carry over progress from a previous save file.
      foreach (Achievement achievement in Achievements)
      {
        achievement.CurrentValue = 0;
        achievement.Completed = false;
      }

      // retrieve saved progress
      var savedProgress = Helper.Data.ReadSaveData<List<AchievementProgress>>(
        "achievement-progress"
        );

      if (savedProgress == null)
      {
        Monitor.Log("No saved achievement progress found.", LogLevel.Info);
        return;
      }
    }

    public void SaveProgress()
    {
      List<AchievementProgress> savedProgress = new List<AchievementProgress>();
      if (savedProgress != null)
      {
        foreach (var achievement in Achievements)
        {
          savedProgress.Add(new AchievementProgress
          {
            Id = achievement.Id,
            CurrentValue = achievement.CurrentValue,
            Completed = achievement.Completed
          });
        }

        Monitor.Log("Saved achievement progress for " + savedProgress.Count + " achievements.", LogLevel.Info);
      }
    }

    public void SetProgress(Achievement achievement, double value)
    {
      if (achievement == null || achievement.Completed)
        return;

      achievement.SetProgress(value);

      if (achievement.Completed)
      {
        Monitor.Log("Unlocked: " + achievement.Name, LogLevel.Info);
      }
    }

    public void IncrementProgress(Achievement achievement, double amount = 1)
    {
      if (achievement == null || achievement.Completed) return;

      achievement.Increment(amount);

      if (achievement.Completed)
      {
        Monitor.Log("Unlocked: " + achievement.Name, LogLevel.Info);
      }
    }

    public void UpdateProgress(string achievementId, double value)
    {
      Achievement achievement = Achievements.Find(
        a => a.Id == achievementId
      );

      if (achievement == null)
        return;

      SetProgress(achievement, value);
    }

    public void CheckCompletion(Achievement achievement)
    {
      if (achievement == null || achievement.Completed)
        return;

      if (achievement.CheckCompletion())
      {
        Monitor.Log("Unlocked: " + achievement.Name, LogLevel.Info);
      }
    }

    public void CompleteAchievement(Achievement achievement)
    {
      if (achievement == null || achievement.Completed)
        return;

      achievement.Completed = true;

      Monitor.Log(
          $"Unlocked: {achievement.Name}!",
          LogLevel.Info
      );
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
            CompleteAchievement(achievement);

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