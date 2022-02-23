using A2.Managers;
using UnityEngine;

namespace A2.States
{
    public class Microbe : TransformAgent
    {
        public int Hunger { get; set; } = 10;
        
        public Microbe TargetMicrobe { get; set; }

        public bool IsHungry => Hunger >= MicrobeManager.MicrobeManagerSingleton.HungerThreshold;

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
            Hunger = Mathf.Max(0, Hunger - 10);
            AddMessage($"Ate {eaten.name}.");
        }

        public void Die()
        {
            AddMessage("Died.");
            Destroy(gameObject);
        }
    }
}