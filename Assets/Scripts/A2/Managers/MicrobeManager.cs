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
            Impress,
            Mated
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
        private int startingHunger = 0;

        [SerializeField]
        [Min(1)]
        private int hungerRestoredFromEating = 100;

        [SerializeField]
        [Min(0)]
        private float floorRadius = 10f;

        [SerializeField]
        [Min(1)]
        private int minMicrobes = 10;

        [SerializeField]
        [Min(2)]
        private int maxMicrobes = 30;

        [SerializeField]
        [Min(0)]
        private float randomSpawnChance = 0.0001f;

        [SerializeField]
        [Min(float.Epsilon)]
        private float minMicrobeSpeed = 5f;

        [SerializeField]
        [Min(float.Epsilon)]
        private float maxMicrobeSpeed = 10f;

        [SerializeField]
        [Min(float.Epsilon)]
        private float minMicrobeLifespan = 20f;

        [SerializeField]
        [Min(float.Epsilon)]
        private float maxMicrobeLifespan = 30f;

        [SerializeField]
        [Min(1)]
        private int maxOffspring = 4;

        [SerializeField]
        [Min(float.Epsilon)]
        private float microbeInteractRadius = 2;

        [SerializeField]
        [Min(float.Epsilon)]
        private float minMicrobeSize = 0.25f;

        [SerializeField]
        [Min(float.Epsilon)]
        private float maxMicrobeSize = 1;

        public int HungerRestoredFromEating => hungerRestoredFromEating;

        public float MicrobeInteractRadius => microbeInteractRadius;

        public Material RedMicrobeMaterial => redMicrobeMaterial;

        public Material BlueMicrobeMaterial => blueMicrobeMaterial;

        public Material GreenMicrobeMaterial => greenMicrobeMaterial;

        public Material YellowMicrobeMaterial => yellowMicrobeMaterial;

        public int HungerThreshold => hungerThreshold;

        public float FloorRadius => floorRadius;

        public int Mate(Microbe parentA, Microbe parentB)
        {
            int born;
            for (born = 0; born < maxOffspring && Agents.Count < maxMicrobes; born++)
            {
                SpawnMicrobe(
                    Random.value <= 0.5f ? parentA.MicrobeType : parentB.MicrobeType,
                    (parentA.transform.position + parentB.transform.position) / 2,
                    Mathf.Clamp((parentA.MoveSpeed + parentB.MoveSpeed) / 2 + Random.value - 0.5f, minMicrobeSpeed, maxMicrobeSpeed),
                    Mathf.Clamp((parentA.LifeSpan + parentB.LifeSpan) / 2 + Random.value - 0.5f, minMicrobeLifespan, maxMicrobeLifespan));
            }

            return born;
        }

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
            SpawnMicrobe(microbeType, position, Random.Range(minMicrobeSpeed, maxMicrobeSpeed), Random.Range(minMicrobeLifespan, maxMicrobeLifespan));
        }

        public void SpawnMicrobe(MicrobeType microbeType, Vector3 position, float moveSpeed, float lifespan)
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
            microbe.LifeSpan = lifespan;
            microbe.AssignMoveSpeed(moveSpeed);

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

            microbes = microbes.Where(m => m.MicrobeType != seeker.MicrobeType).ToArray();

            return microbes.Length == 0 ? null : microbes.OrderBy(m => Vector3.Distance(seeker.transform.position, m.transform.position)).First();
        }

        public Microbe FindMate(Microbe seeker)
        {
            Microbe[] microbes = Agents.Where(a => a is Microbe m && m != seeker && m.IsAdult && m.State.GetType() == typeof(MicrobeSeekingMateState) && !seeker.RejectedBy.Contains(m)).Cast<Microbe>().ToArray();
            if (microbes.Length == 0)
            {
                return null;
            }
            
            microbes = seeker.MicrobeType switch
            {
                MicrobeType.Red => microbes.Where(m => m.MicrobeType == MicrobeType.Red || m.MicrobeType == MicrobeType.Yellow).ToArray(),
                MicrobeType.Blue => microbes.Where(m => m.MicrobeType == MicrobeType.Blue || m.MicrobeType == MicrobeType.Green).ToArray(),
                MicrobeType.Green => microbes.Where(m => m.MicrobeType == MicrobeType.Blue || m.MicrobeType == MicrobeType.Green).ToArray(),
                _ => microbes.Where(m => m.MicrobeType == MicrobeType.Red || m.MicrobeType == MicrobeType.Yellow).ToArray()
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
                if (!(Agents[index] is Microbe microbe))
                {
                    continue;
                }

                microbe.ElapsedLifespan += Time.deltaTime;

                if (Vector3.Distance(Agents[index].transform.position, Vector3.zero) <= floorRadius && microbe.ElapsedLifespan < microbe.LifeSpan)
                {
                    if (Agents[index].Visuals != null)
                    {
                        float scale = (microbe.ElapsedLifespan / microbe.LifeSpan) * (maxMicrobeSize - minMicrobeSize) + minMicrobeSize;
                        Agents[index].Visuals.localScale = new Vector3(scale, scale, scale);
                    }

                    continue;
                }

                microbe.Die();
                index--;
            }

            while (Agents.Count < minMicrobes)
            {
                SpawnMicrobe();
            }

            for (int i = Agents.Count; i < maxMicrobes; i++)
            {
                if (Random.value <= randomSpawnChance)
                {
                    SpawnMicrobe();
                }
            }
        }
    }
}