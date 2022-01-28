namespace EasyAI.Components
{
    public abstract class DataPiece
    {
        public virtual string DetailsDisplay()
        {
            return null;
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}