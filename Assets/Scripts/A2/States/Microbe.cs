using UnityEngine;

namespace A2.States
{
    public class Microbe : TransformAgent
    {
        public int hunger = 10;

        public void Eat(Agent eaten)
        {
            hunger = Mathf.Max(0, hunger - 10);
            AddMessage($"Ate {eaten.name}.");
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}