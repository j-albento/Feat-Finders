using System;

namespace FeatFinders.Achievements
{
    public class Achievement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }


        public string CollectionType { get; set; }
        public string EventKey { get; set; }
        public string CounterType { get; set; }
        public string Mode { get; set; }


        public int? HeartRequirement { get; set; }
        public int? NpcCount { get; set; }
        public int? AmountPerCrop { get; set; }
        public int? TargetValue { get; set; }
        public int? RequiredLevel { get; set; }

        public double CurrentValue { get; set; } = 0;
        public bool Completed { get; set; } = false;

        public Achievement() { }

        // Used when an action adds to a player's progress
        public void Increment(double amount = 1)
        {
            if (Completed || amount <= 0)
                return;

            SetProgress(CurrentValue + amount);
        }

        public bool CheckCompletion()
        {
            if (Completed)
                return true;

            if (TargetValue.HasValue &&
            CurrentValue >= TargetValue.Value)
            {
                Completed = true;
            }
            return Completed;
        }

        public int GetProgressPercentage()
        {
            if (Completed)
            {
                return 100;
            }
            if (!TargetValue.HasValue || TargetValue.Value <= 0)
            {
                return 0;
            }

            double percent = (CurrentValue / TargetValue.Value) * 100;

            // maximum of 100 to prevent UI overflow
            return (int)Math.Min(100, Math.Ceiling(percent));
        }

        // Used when a players current total is already known
        public void SetProgress(double value)
        {
            if (Completed)
                return;

            CurrentValue = Math.Max(0, value);

            if (TargetValue.HasValue)
            {
                CurrentValue = Math.Min(CurrentValue, TargetValue.Value);

            }
            CheckCompletion();
        }


    }
}