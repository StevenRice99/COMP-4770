using EasyAI.Percepts;

namespace A1.Percepts
{
    public class DirtyPercept : Percept
    {
        public Floor Floor;

        public bool IsDirty => Floor != null && Floor.State >= Floor.DirtLevel.Dirty;

        public override string DetailsDisplay()
        {
            return IsDirty ? "Dirty." : "Clean.";
        }
    }
}