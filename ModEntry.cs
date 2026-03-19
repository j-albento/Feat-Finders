using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using FeatFinders.Achievements;

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

    AchievementManager AchievementManager;
    public override void Entry(IModHelper helper)
    {
      AchievementManager = new AchievementManager(this.Monitor, helper);

      // helper.Events.Input.ButtonPressed += this.OnButtonPressed;
      helper.ConsoleCommands.Add("check_achievements", "Prints the currently loaded achievements.\n\nUsage: check_achievements", this.PrintAchievements);
    }


    /*********
    ** Private methods
    *********/
    /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
      // ignore if player hasn't loaded a save yet
      if (!Context.IsWorldReady)
        return;

      // print button presses to the console window
      this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
    }


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