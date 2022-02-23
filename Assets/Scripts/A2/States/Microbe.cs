using System.Collections.Generic;
using System.Linq;
using A2.Managers;
using UnityEngine;

namespace A2.States
{
    public class Microbe : TransformAgent
    {
        public int Hunger { get; set; } = 10;
        
        public float LifeSpan { get; set; }
        
        public float ElapsedLifespan { get; set; }
        
        public Microbe TargetMicrobe { get; set; }

        public List<Microbe> RejectedBy { get; } = new List<Microbe>();

        public bool IsHungry => Hunger >= MicrobeManager.MicrobeManagerSingleton.HungerThreshold;

        public bool IsAdult => ElapsedLifespan >= LifeSpan / 2;

        private MicrobeManager.MicrobeType _microbeType;

        public MicrobeManager.MicrobeType MicrobeType
        {
            get => _microbeType;
            set
            {
                _microbeType = value;

                MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
                if (meshRenderer == null)
                {
                    return;
                }

                meshRenderer.material = _microbeType switch
                {
                    MicrobeManager.MicrobeType.Red => MicrobeManager.MicrobeManagerSingleton.RedMicrobeMaterial,
                    MicrobeManager.MicrobeType.Blue => MicrobeManager.MicrobeManagerSingleton.BlueMicrobeMaterial,
                    MicrobeManager.MicrobeType.Green => MicrobeManager.MicrobeManagerSingleton.GreenMicrobeMaterial,
                    _ => MicrobeManager.MicrobeManagerSingleton.YellowMicrobeMaterial
                };
            }
        }

        public void Eat(Agent eaten)
        {
            Hunger = Mathf.Max(0, Hunger - MicrobeManager.MicrobeManagerSingleton.HungerRestoredFromEating);
            AddMessage($"Ate {eaten.name}.");
        }

        public void Die()
        {
            AddMessage("Died.");
            Destroy(gameObject);
        }
        
        /// <summary>
        /// Override for custom detail rendering on the automatic GUI.
        /// </summary>
        /// <param name="x">X rendering position. In most cases this should remain unchanged.</param>
        /// <param name="y">Y rendering position. Update this with every component added and return it.</param>
        /// <param name="w">Width of components. In most cases this should remain unchanged.</param>
        /// <param name="h">Height of components. In most cases this should remain unchanged.</param>
        /// <param name="p">Padding of components. In most cases this should remain unchanged.</param>
        /// <returns>The updated Y position after all custom rendering has been done.</returns>
        public override float DisplayDetails(float x, float y, float w, float h, float p)
        {
            y = AgentManager.NextItem(y, h, p);
            AgentManager.GuiBox(x, y, w, h, p, 2);

            AgentManager.GuiLabel(x, y, w, h, p, $"Hunger: {Hunger} | " + (IsHungry ? "Hungry" : "Not Hungry"));
            y = AgentManager.NextItem(y, h, p);

            AgentManager.GuiLabel(x, y, w, h, p, $"Lifespan: {ElapsedLifespan} / {LifeSpan}");
            
            return y;
        }

        protected override void OnDestroy()
        {
            Microbe[] microbes = AgentManager.Singleton.Agents.Where(a => a is Microbe m && m != this).Cast<Microbe>().ToArray();
            if (microbes.Length > 0)
            {
                foreach (Microbe microbe in microbes)
                {
                    microbe.RejectedBy.Remove(microbe);
                }
            }
            
            base.OnDestroy();
        }
    }
}