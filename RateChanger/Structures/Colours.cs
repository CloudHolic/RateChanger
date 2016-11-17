namespace RateChanger.Structures
{
    public class Colours
    {
        public Colours()
        {
            Number = 0;

            Red = 0;
            Green = 0;
            Blue = 0;
        }

        public Colours(int num, int r, int g, int b)
        {
            Number = num;

            Red = r;
            Green = g;
            Blue = b;
        }

        public Colours(Colours prevColours)
        {
            Number = prevColours.Number;

            Red = prevColours.Red;
            Green = prevColours.Green;
            Blue = prevColours.Blue;
        }

        public int Number { get; set; }

        public int Red { get; set; }

        public int Green { get; set; }

        public int Blue { get; set; }
    }
}
