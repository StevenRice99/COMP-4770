using A2.Agents;

namespace A2.Pickups
{
    public class NeverHungryPickup : MicrobeBasePickup
    {
        protected override void Execute(Microbe microbe)
        {
            microbe.AddMessage("Powered up - will not be hungry for eternity!");
        }
    }
}