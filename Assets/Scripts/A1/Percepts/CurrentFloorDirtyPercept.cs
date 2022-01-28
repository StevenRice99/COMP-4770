using EasyAI.Percepts;

namespace A1.Percepts
{
    public class CurrentFloorDirtyPercept : Percept
    {
        public bool IsDirty;

        public override string DetailsDisplay()
        {
            return IsDirty ? "Dirty." : "Clean.";
        }
    }
}