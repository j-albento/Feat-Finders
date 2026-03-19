
/**
    This class represents one achivement. Its responsibilities include storing its id
    and description, track current progress, track target for completion, and determine if its completed.
**/

namespace FeatFinders.Achievements
{
    public class Achievement
    {
        // Properties
        private int ID;
        private string Name;
        private string Description;
        private int TargetValue;
        private double CurrentValue; // e.g. 50/100 fish
        private bool Completed;


        public Achievement(int id, string name, string description, int targetValue, double currentValue, bool completed)
        {
            ID = id;
            Name = name;
            Description = description;
            TargetValue = targetValue;
            CurrentValue = currentValue;
            Completed = completed;
        }

        // adds progress to currentValue
        private Increment()
        {
            CurrentValue++;
        }

        // checks if a achievement has reached its target value
        private bool IsCompleted()
        {

            if (CurrentValue == TargetValue)
            {
                Completed = true;
            }
            else
            {
                Completed = false;
            }
            return Completed;
        }

        // returns progress as a number between 0-100 for UI
        private int GetProgressPercentage()
        {
            int progressPercent = (int)Math.Ceiling(CurrentValue / TargetValue * 100);
            return progressPercent;
        }



        /**
            Getter and setters
        **/
        protected int ID { get; set; }
        protected string Name { get; set; }
        protected string Description { get; set; }
        protected int TargetValue { get; set; }
        protected double CurrentValue { get; set; }
        protected bool Completed { get; set; }

    }
}