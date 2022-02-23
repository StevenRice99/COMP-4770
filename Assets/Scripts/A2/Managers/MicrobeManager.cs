using System.Linq;
using A2.States;
using UnityEngine;
using Random = UnityEngine.Random;

namespace A2.Managers
{
    public class MicrobeManager : AgentManager
    {
        public enum MicrobeType
        {
            Red = 0,
            Blue,
            Green,
            Yellow
        }
        
        public enum MicrobeEvents
        {
            Eaten = 0,
            Attracted,
            Rejected
        }
        
        public static MicrobeManager MicrobeManagerSingleton => Singleton as MicrobeManager;
        
        [SerializeField]
        private GameObject microbePrefab;

        [SerializeField]
        private Material redMicrobeMaterial;

        [SerializeField]
        private Material blueMicrobeMaterial;

        [SerializeField]
        private Material greenMicrobeMaterial;

        [SerializeField]
        private Material yellowMicrobeMaterial;

        [SerializeField]
        [Min(1)]
        private int hungerThreshold = 100;

        [SerializeField]
        [Min(0)]
        private int startingHunger = 10;

        [SerializeField]
        [Min(0)]
        private float floorRadius = 10f;

        [SerializeField]
        [Min(1)]
        private int minMicrobes = 5;

        [SerializeField]
        [Min(2)]
        private int maxMicrobes = 20;

        public Material RedMicrobeMaterial => redMicrobeMaterial;

        public Material BlueMicrobeMaterial => blueMicrobeMaterial;

        public Material GreenMicrobeMaterial => greenMicrobeMaterial;

        public Material YellowMicrobeMaterial => yellowMicrobeMaterial;

        public int HungerThreshold => hungerThreshold;

        public void SpawnMicrobe()
        {
            SpawnMicrobe((MicrobeType) Random.Range((int) MicrobeType.Red, (int) MicrobeType.Yellow + 1));
        }

        public void SpawnMicrobe(MicrobeType microbeType)
        {
            Vector3 position = Random.insideUnitSphere * floorRadius;
            position = new Vector3(position.x, 0, position.z);
            
            SpawnMicrobe(microbeType, position);
        }

        public void SpawnMicrobe(MicrobeType microbeType, Vector3 position)
        {
            if (Agents.Count >= maxMicrobes)
            {
                return;
            }
            
            GameObject go = Instantiate(microbePrefab, position, Quaternion.identity);
            Microbe microbe = go.GetComponent<Microbe>();
            if (microbe == null)
            {
                return;
            }

            microbe.MicrobeType = microbeType;
            microbe.Hunger = startingHunger;

            string n = microbeType switch
            {
                MicrobeType.Red => "Red",
                MicrobeType.Blue => "Blue",
                MicrobeType.Green => "Green",
                _ => "Yellow"
            };

            Agent[] coloredMicrobes = Agents.Where(a => a is Microbe m && m.MicrobeType == microbeType && m != microbe).ToArray();
            if (coloredMicrobes.Length == 0)
            {
                microbe.name = $"{n} 1";
            }

            for (int i = 1;; i++)
            {
                if (coloredMicrobes.Any(m => m.name == $"{n} {i}"))
                {
                    continue;
                }

                n = $"{n} {i}";
                microbe.name = n;
                break;
            }

            SortAgents();
            
            AddGlobalMessage($"Spawned microbe {n}.");
        }

        public Microbe FindFood(Microbe seeker)
        {
            Microbe[] microbes = Agents.Where(a => a is Microbe m && m != seeker).Cast<Microbe>().ToArray();
            if (microbes.Length == 0)
            {
                return null;
            }

            microbes = seeker.MicrobeType switch
            {
                MicrobeType.Red => microbes.Where(m => m.MicrobeType == MicrobeType.Blue).ToArray(),
                MicrobeType.Blue => microbes.Where(m => m.MicrobeType == MicrobeType.Yellow).ToArray(),
                MicrobeType.Green => microbes.Where(m => m.MicrobeType == MicrobeType.Red).ToArray(),
                _ => microbes.Where(m => m.MicrobeType == MicrobeType.Green).ToArray()
            };

            return microbes.Length == 0 ? null : microbes.OrderBy(m => Vector3.Distance(seeker.transform.position, m.transform.position)).First();
        }

        protected override void Start()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(floor.GetComponent<Collider>());
            floor.transform.position = new Vector3(0, -1, 0);
            floor.transform.localScale = new Vector3(floorRadius * 2, 1, floorRadius * 2);
            floor.name = "Floor";

            for (int i = 0; i < minMicrobes; i++)
            {
                SpawnMicrobe();
            }
            
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            for (int index = 0; index < Agents.Count; index++)
            {
                if (!(Agents[index] is Microbe microbe) || Vector3.Distance(Agents[index].transform.position, Vector3.zero) <= floorRadius)
                {
                    continue;
                }
                
                microbe.Die();
                index--;
            }

            while (Agents.Count < minMicrobes)
            {
                SpawnMicrobe();
            }
        }
    }
}