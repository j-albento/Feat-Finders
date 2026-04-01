using System.Collections.Generic;
using StardewModdingAPI;
using System.Text.Json.Serialization;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.ComponentModel;
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
        Monitor.Log($"Successfully loaded {Achievements.Count} achievements.", LogLevel.Info);
      }
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
  }
}