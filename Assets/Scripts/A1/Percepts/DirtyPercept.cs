namespace A1.Percepts
{
    /// <summary>
    /// Hold the dirt state of the current tile the agent is on.
    /// </summary>
    public class DirtyPercept : Percept
    {
        /// <summary>
        /// The floor closest to the agent.
        /// </summary>
        public Floor Floor;

        /// <summary>
        /// Getter for if the closest floor tile is dirty or not.
        /// </summary>
        public bool IsDirty => Floor != null && Floor.State >= Floor.DirtLevel.Dirty;

        /// <summary>
        /// Display the details of the percept.
        /// </summary>
        /// <returns>String with the details of the percept.</returns>
        public override string DetailsDisplay()
        {
            return IsDirty ? "Dirty." : "Clean.";
        }
    }
}