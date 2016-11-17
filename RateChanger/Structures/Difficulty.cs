namespace RateChanger.Structures
{
    public class Difficulty
    {
        public Difficulty()
        {
            HPDrainRate = 5;
            CircleSize = 4;
            OverallDifficulty = 5;
            ApproachRate = 5;
            SliderMultiplier = 0;
            SliderTickRate = 0;
        }

        public Difficulty(double hp, double cs, double od, double ar, double multiplier, double tickrate)
        {
            HPDrainRate = hp;
            CircleSize = cs;
            OverallDifficulty = od;
            ApproachRate = ar;
            SliderMultiplier = multiplier;
            SliderTickRate = tickrate;
        }

        public Difficulty(Difficulty prevDifficulty)
        {
            HPDrainRate = prevDifficulty.HPDrainRate;
            CircleSize = prevDifficulty.CircleSize;
            OverallDifficulty = prevDifficulty.OverallDifficulty;
            ApproachRate = prevDifficulty.ApproachRate;
            SliderMultiplier = prevDifficulty.SliderMultiplier;
            SliderTickRate = prevDifficulty.SliderTickRate;
        }

        // ReSharper disable once InconsistentNaming
        public double HPDrainRate { get; set; }

        public double CircleSize { get; set; }

        public double OverallDifficulty { get; set; }

        public double ApproachRate { get; set; }

        public double SliderMultiplier { get; set; }

        public double SliderTickRate { get; set; }
    }
}
