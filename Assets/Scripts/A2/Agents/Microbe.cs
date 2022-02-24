using System.Collections;
using System.Collections.Generic;
using System.Linq;
using A2.Managers;
using A2.Pickups;
using A2.States;
using UnityEngine;

namespace A2.Agents
{
    public class Microbe : TransformAgent
    {
        [SerializeField]
        private MeshRenderer stateVisualization;
        
        public int Hunger { get; set; }
        
        public float LifeSpan { get; set; }

        public float DetectionRange { get; set; }
        
        public float ElapsedLifespan { get; set; }
        
        public Microbe TargetMicrobe { get; set; }
        
        public MicrobeBasePickup TargetPickup { get; set; }
        
        public bool DidMate { get; set; }

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
                    MicrobeManager.MicrobeType.Orange => MicrobeManager.MicrobeManagerSingleton.OrangeMicrobeMaterial,
                    MicrobeManager.MicrobeType.Yellow => MicrobeManager.MicrobeManagerSingleton.YellowMicrobeMaterial,
                    MicrobeManager.MicrobeType.Green => MicrobeManager.MicrobeManagerSingleton.GreenMicrobeMaterial,
                    MicrobeManager.MicrobeType.Blue => MicrobeManager.MicrobeManagerSingleton.BlueMicrobeMaterial,
                    MicrobeManager.MicrobeType.Purple => MicrobeManager.MicrobeManagerSingleton.PurpleMicrobeMaterial,
                    _ => MicrobeManager.MicrobeManagerSingleton.PinkMicrobeMaterial
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

        public void SetStateVisual(State state)
        {
            if (stateVisualization == null)
            {
                return;
            }
            
            if (state as MicrobeSleepingState)
            {
                stateVisualization.material = MicrobeManager.MicrobeManagerSingleton.SleepingIndicatorMaterial;
                return;
            }
            
            if (state as MicrobeSeekingFoodState)
            {
                stateVisualization.material = MicrobeManager.MicrobeManagerSingleton.FoodIndicatorMaterial;
                return;
            }
            
            if (state as MicrobeSeekingMateState)
            {
                stateVisualization.material = MicrobeManager.MicrobeManagerSingleton.MateIndicatorMaterial;
            }
            
            if (state as MicrobeSeekingPickupState)
            {
                stateVisualization.material = MicrobeManager.MicrobeManagerSingleton.PickupIndicatorMaterial;
            }
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
            AgentManager.GuiBox(x, y, w, h, p, 3);

            AgentManager.GuiLabel(x, y, w, h, p, $"Hunger: {Hunger} | " + (IsHungry ? "Hungry" : "Not Hungry"));
            y = AgentManager.NextItem(y, h, p);

            AgentManager.GuiLabel(x, y, w, h, p, $"Lifespan: {ElapsedLifespan} / {LifeSpan} | " + (IsAdult ? "Adult" : "Infant"));
            y = AgentManager.NextItem(y, h, p);
            
            AgentManager.GuiLabel(x, y, w, h, p, $"Mating: " + (IsAdult && !IsHungry ? "Mating | " + (TargetMicrobe == null ? "Searching..." : $"With {TargetMicrobe.name}") : "No"));
            
            return y;
        }

        protected override void Start()
        {
            base.Start();
            
            SetStateVisual(State);
        }
    }
}