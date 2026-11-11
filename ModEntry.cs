using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using FeatFinders.Achievements;
using System.Runtime.CompilerServices;
using StardewModdingAPI.Framework.ModLoading.Rewriters.StardewValley_1_6;

namespace FeatFinders
{
  /// <summary>The mod entry point.</summary>
  internal sealed class ModEntry : Mod
  {
    /*********
    ** Public methods
    *********/
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>

    public AchievementManager AchievementManager;

    public override void Entry(IModHelper helper)
    {
      AchievementManager = new AchievementManager(Monitor, helper);

      // helper.Events.Input.ButtonPressed += this.OnButtonPressed;
      helper.ConsoleCommands.Add(
        "check_achievements",
        "Prints the currently loaded achievements.\n\nUsage: check_achievements",
        PrintAchievements
      );

      helper.ConsoleCommands.Add("check_fishing_stats", "Displays the player's fishing statistics.", CheckFishingStats);
      helper.ConsoleCommands.Add(
          "inspect_fish",
          "Displays the player's fishing records.",
          CheckFishingStats
      );

      // subscribed
      helper.Events.GameLoop.UpdateTicked += OnMoneyGained;
      helper.Events.Player.LevelChanged += OnLevelChanged;
      helper.Events.GameLoop.UpdateTicked += OnEventCompleted;
      helper.Events.GameLoop.UpdateTicked += OnFriendCountCompleted;
      helper.Events.Player.Warped += OnDepthReached;
      helper.Events.GameLoop.UpdateTicked += OnFishingAchievementsUpdate;
      helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
      helper.Events.GameLoop.Saving += OnSaving;
    }


    // helper method to check if a player has loaded into a relevant save
    private static bool IsWorldReady()
    {
      return Context.IsWorldReady;
    }


    private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
    {
      if (!Context.IsMainPlayer)
        return;

      AchievementManager.LoadProgress();
    }

    private void OnSaving(object sender, SavingEventArgs e)
    {
      // Only the main player should save progress to avoid conflicts in multiplayer
      if (!Context.IsMainPlayer)
        return;

      AchievementManager.SaveProgress();
    }


    /*********
    ** Private methods
    *********/
    /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    /// 
    // private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    // {
    //   // ignore if player hasn't loaded a save yet
    //   if (!IsWorldReady()) return;

    //   // print button presses to the console window
    //   Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
    // }



    // tick based method used to track money gained by player, fires only 
    private void OnMoneyGained(object sender, UpdateTickedEventArgs e)
    {
      if (!IsWorldReady()) return;

      if (e.IsMultipleOf(36000))
      {
        // fetch players total lifetime money earned; NOT current balance!!!!
        uint lifeTimeEarnings = Game1.player.totalMoneyEarned;

        foreach (Achievement achievement in AchievementManager.Achievements)
        {
          if (achievement.Completed) continue; // continue to the next achievement

          if (achievement.Type == "earn")
          {
            // update achievements current progress value to the players lifetime earnings
            AchievementManager.SetProgress(achievement, lifeTimeEarnings);
          }
        }
      }
    }


    private void OnLevelChanged(object sender, LevelChangedEventArgs e)
    {
      // Check if the game world is loaded before accessing player data
      if (!IsWorldReady()) return;


      // get the player and their previous skill level and their new skill level
      foreach (Achievement achievement in AchievementManager.Achievements)
      {
        if (achievement.Completed) continue; // if the achievement is already completed

        // Handle single skill achievements
        if (achievement.Type == "skill-level" &&
        e.NewLevel >= achievement.TargetValue)
        {
          AchievementManager.CompleteAchievement(achievement);
        }
        // handle all skill achievements
        else if (achievement.Type == "all-skills" &&
        achievement.RequiredLevel.HasValue &&
        HasAllSkills(achievement.RequiredLevel.Value))
        {
          AchievementManager.SetProgress(achievement, achievement.RequiredLevel.Value);
        }
      }
    }


    // handle an event achievement being completed
    private void OnEventCompleted(object sender, UpdateTickedEventArgs e)
    {
      if (!IsWorldReady()) return;

      if (e.IsMultipleOf(3600))
      {
        foreach (Achievement achievement in AchievementManager.Achievements)
        {
          if (achievement.Completed) continue;

          if (achievement.Type == "event" &&
              !string.IsNullOrEmpty(achievement.EventKey))
          {
            // Check if the player's seen events list contains the target EventKey
            if (Game1.player.eventsSeen.Contains(achievement.EventKey))
            {
              AchievementManager.SetProgress(achievement, 1);

            }
          }
        }
      }
    }

    // get the friendship count of x NPC
    private void OnFriendCountCompleted(object sender, UpdateTickedEventArgs e)
    {
      if (!IsWorldReady()) return;

      if (e.IsMultipleOf(3600))
      {
        foreach (Achievement achievement in AchievementManager.Achievements)
        {
          if (achievement.Completed) continue;

          if (achievement.Type == "friendship-count")
          {
            int requiredPoints = (int)achievement.HeartRequirement * 250;
            int npcCount = 0;

            foreach (var friendship in Game1.player.friendshipData.Values)
            {
              if (friendship.Points >= requiredPoints)
              {
                npcCount++;
              }
            }
            if (npcCount >= achievement.NpcCount)
            {
              AchievementManager.CompleteAchievement(achievement);
              Monitor.Log($"Unlocked: {achievement.Name}!", LogLevel.Info);
            }

          }
        }
      }
    }

    // handles achievement for reaching X levels in the mines
    private void OnDepthReached(object sender, WarpedEventArgs e)
    {
      // handle cases for types of mines (regular or dangerous)
      if (!IsWorldReady()) return;

      if (e.NewLocation is StardewValley.Locations.MineShaft mine)
      {
        string currentMineType = "regular";
        int currentDepth = mine.mineLevel;

        if (currentDepth > 120)
        {
          currentMineType = "skull_cavern";
          currentDepth -= 120;
        }

        if (mine.GetAdditionalDifficulty() > 0)
        {
          currentMineType += "_dangerous";
        }

        // get the players current mine level
        foreach (Achievement achievement in AchievementManager.Achievements)
        {
          if (achievement.Completed) continue;

          if (achievement.Type != "depth" ||
              achievement.Mode != currentMineType)
          {

            continue;
          }

          if (achievement.TargetValue.HasValue)
          {
            continue;
          }

          AchievementManager.SetProgress(achievement, currentDepth);
        }
      }
    }


    private bool HasAllSkills(int targetLevel)
    {
      for (int i = 0; i < 5; i++)
      {
        if (Game1.player.GetSkillLevel(i) < targetLevel)
        {
          return false;
        }
      }
      return true;
    }

    private void CheckFishingStats(string command, string[] args)
    {
      if (!IsWorldReady())
        return;

      uint totalFishCaught = Game1.player.stats.FishCaught;

      Monitor.Log(
          $"Total fish caught: {totalFishCaught}",
          LogLevel.Info
      );

      var fishRecords = Game1.player.fishCaught;

      int uniqueFishCount = 0;

      foreach (var fish in fishRecords.Pairs)
      {
        uniqueFishCount++;

        string fishId = fish.Key;
        string fishName = ItemRegistry.GetData(fishId).DisplayName;

        Monitor.Log(
            $"Name: {fishName} | " +
            $"ID: {fishId} | " +
            $"Quantity: {fish.Value[0]} | " +
            $"Largest Catch: {fish.Value[1]} in\" ",
            LogLevel.Info
        );
      }

      Monitor.Log(
          $"Different fish IDs recorded: {uniqueFishCount}",
          LogLevel.Info
      );
    }

    private void UpdateFishingAchievements()
    {
      if (!IsWorldReady())
        return;

      var fishRecords = Game1.player.fishCaught;

      int uniqueFishCount = fishRecords.Pairs.Count();

      AchievementManager.UpdateUniqueFishProgress(uniqueFishCount);

      uint totalFishCaught = Game1.player.stats.FishCaught;

      AchievementManager.UpdateTotalFishProgress(totalFishCaught);
    }

    private void OnFishingAchievementsUpdate(
    object sender,
    UpdateTickedEventArgs e)
    {
      if (!IsWorldReady())
        return;

      if (e.IsMultipleOf(3600))
      {
        UpdateFishingAchievements();
      }
    }












    // helper method to make sure all achievements are accounted for
    private void PrintAchievements(string command, string[] args)
    {
      var achievements = AchievementManager.Achievements;

      if (achievements == null || achievements.Count == 0)
      {
        Monitor.Log("No achievements are currently loaded.", LogLevel.Warn);
        return;
      }

      Monitor.Log($"--- Loaded {achievements.Count} Achievements ---", LogLevel.Info);

      // Loop through and print out some details to verify the data mapped correctly
      foreach (var ach in achievements)
      {
        Monitor.Log($"ID: {ach.Id} | Name: {ach.Name} | Type: {ach.Type} | Target: {ach.TargetValue}", LogLevel.Debug);
      }
    }


  }
}


