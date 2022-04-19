namespace Project.Positions
{
    public class StrategicPoint : Position
    {
        public bool defensive = true;

        public bool Open => Count == 0;
    }
}