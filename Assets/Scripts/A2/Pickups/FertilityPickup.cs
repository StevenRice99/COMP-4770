using A2.Agents;

namespace A2.Pickups
{
    public class FertilityPickup : MicrobeBasePickup
    {
        protected override void Execute(Microbe microbe)
        {
            microbe.AddMessage("Powered up -  can now mate again!");
            microbe.DidMate = false;
        }
    }
}