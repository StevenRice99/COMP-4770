using A2.Agents;

namespace A2.Pickups
{
    public class RejuvenatePickup : MicrobeBasePickup
    {
        protected override void Execute(Microbe microbe)
        {
            microbe.AddMessage("Powered up - has extended life and is now a young adult again!");
            microbe.ElapsedLifespan = microbe.LifeSpan / 2;
        }
    }
}