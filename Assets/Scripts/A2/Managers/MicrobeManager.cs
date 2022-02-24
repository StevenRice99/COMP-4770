using System.Linq;
using A2.Agents;
using A2.Pickups;
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
            Orange,
            Yellow,
            Green,
            Blue,
            Purple,
            Pink
        }
        
        public enum MicrobeEvents
        {
            Eaten = 0,
            Impress,
            Mate
        }
        
        public static MicrobeManager MicrobeManagerSingleton => Singleton as MicrobeManager;
        
        [SerializeField]
        private GameObject microbePrefab;

        [SerializeField]
        private GameObject fertilityPickupPrefab;

        [SerializeField]
        private GameObject neverHungryPickupPrefab;

        [SerializeField]
        private GameObject offspringPickupPrefab;

        [SerializeField]
        private GameObject rejuvenatePickupPrefab;

        [SerializeField]
        private GameObject spawnParticlesPrefab;

        [SerializeField]
        private GameObject deathParticlesPrefab;

        [SerializeField]
        private GameObject mateParticlesPrefab;

        [SerializeField]
        private GameObject pickupParticlesPrefab;

        [SerializeField]
        private Material floorMaterial;

        [SerializeField]
        private Material redMicrobeMaterial;

        [SerializeField]
        private Material orangeMicrobeMaterial;

        [SerializeField]
        private Material yellowMicrobeMaterial;

        [SerializeField]
        private Material greenMicrobeMaterial;

        [SerializeField]
        private Material blueMicrobeMaterial;

        [SerializeField]
        private Material purpleMicrobeMaterial;

        [SerializeField]
        private Material pinkMicrobeMaterial;

        [SerializeField]
        private Material sleepingIndicatorMaterial;

        [SerializeField]
        private Material foodIndicatorMaterial;

        [SerializeField]
        private Material mateIndicatorMaterial;

        [SerializeField]
        private Material pickupIndicatorMaterial;

        [SerializeField]
        private int startingHunger = -100;

        [SerializeField]
        private int maxHunger = 200;

        [SerializeField]
        [Min(1)]
        private int hungerRestoredFromEating = 100;

        [SerializeField]
        [Min(0)]
        private float floorRadius = 10f;

        [SerializeField]
        [Min(2)]
        private int minMicrobes = 10;

        [SerializeField]
        [Min(2)]
        public int maxMicrobes = 30;

        [SerializeField]
        [Min(1)]
        private int activePickups = 5;

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
        private float microbeInteractRadius = 1;

        [SerializeField]
        [Min(float.Epsilon)]
        private float minMicrobeSize = 0.25f;

        [SerializeField]
        [Min(float.Epsilon)]
        private float maxMicrobeSize = 1;

        [SerializeField]
        [Min(0)]
        private float minMicrobeDetectionRange = 5;
        
        [SerializeField]
        [Min(0)]
        public float hungerChance = 0.05f;

        [SerializeField]
        [Min(0)]
        public float rejectionChance = 0.5f;

        [SerializeField]
        [Min(0)]
        public float rejectionResetTime = 5f;

        public int HungerRestoredFromEating => hungerRestoredFromEating;

        public float MicrobeInteractRadius => microbeInteractRadius;

        public Material RedMicrobeMaterial => redMicrobeMaterial;

        public Material OrangeMicrobeMaterial => orangeMicrobeMaterial;

        public Material YellowMicrobeMaterial => yellowMicrobeMaterial;

        public Material GreenMicrobeMaterial => greenMicrobeMaterial;

        public Material BlueMicrobeMaterial => blueMicrobeMaterial;

        public Material PurpleMicrobeMaterial => purpleMicrobeMaterial;

        public Material PinkMicrobeMaterial => pinkMicrobeMaterial;

        public Material SleepingIndicatorMaterial => sleepingIndicatorMaterial;

        public Material FoodIndicatorMaterial => foodIndicatorMaterial;

        public Material MateIndicatorMaterial => mateIndicatorMaterial;

        public Material PickupIndicatorMaterial => pickupIndicatorMaterial;

        public GameObject SpawnParticlesPrefab => spawnParticlesPrefab;

        public GameObject DeathParticlesPrefab => deathParticlesPrefab;

        public GameObject MateParticlesPrefab => mateParticlesPrefab;

        public GameObject PickupParticlesPrefab => pickupParticlesPrefab;

        public int MaxHunger => maxHunger;

        public float FloorRadius => floorRadius;

        public int Mate(Microbe parentA, Microbe parentB)
        {
            int born;
            Vector3 position = (parentA.transform.position + parentB.transform.position) / 2;
            for (born = 0; born < maxOffspring && Agents.Count < maxMicrobes; born++)
            {
                SpawnMicrobe(
                    Random.value <= 0.5f ? parentA.MicrobeType : parentB.MicrobeType,
                    position,
                    Mathf.Clamp((parentA.MoveSpeed + parentB.MoveSpeed) / 2 + Random.value - 0.5f, minMicrobeSpeed, maxMicrobeSpeed),
                    Mathf.Clamp((parentA.LifeSpan + parentB.LifeSpan) / 2 + Random.value - 0.5f, minMicrobeLifespan, maxMicrobeLifespan),
                    Mathf.Clamp((parentA.DetectionRange + parentB.DetectionRange) / 2 + Random.value - 0.5f, minMicrobeDetectionRange, floorRadius * 2));
            }

            if (born > 0)
            {
                Instantiate(MateParticlesPrefab, position, Quaternion.Euler(270, 0, 0));
            }

            return born;
        }

        public Microbe FindFood(Microbe seeker)
        {
            Microbe[] microbes = Agents.Where(a => a is Microbe m && m != seeker && Vector3.Distance(seeker.transform.position, a.transform.position) < seeker.DetectionRange).Cast<Microbe>().ToArray();
            if (microbes.Length == 0)
            {
                return null;
            }
            
            microbes = seeker.MicrobeType switch
            {
                MicrobeType.Red => microbes.Where(m => m.MicrobeType != MicrobeType.Red && m.MicrobeType != MicrobeType.Orange && m.MicrobeType != MicrobeType.Pink).ToArray(),
                MicrobeType.Orange => microbes.Where(m => m.MicrobeType != MicrobeType.Orange && m.MicrobeType != MicrobeType.Yellow && m.MicrobeType != MicrobeType.Red).ToArray(),
                MicrobeType.Yellow => microbes.Where(m => m.MicrobeType != MicrobeType.Yellow && m.MicrobeType != MicrobeType.Green && m.MicrobeType != MicrobeType.Orange).ToArray(),
                MicrobeType.Green => microbes.Where(m => m.MicrobeType != MicrobeType.Green && m.MicrobeType != MicrobeType.Blue && m.MicrobeType != MicrobeType.Yellow).ToArray(),
                MicrobeType.Blue => microbes.Where(m => m.MicrobeType != MicrobeType.Blue && m.MicrobeType != MicrobeType.Purple && m.MicrobeType != MicrobeType.Green).ToArray(),
                MicrobeType.Purple => microbes.Where(m => m.MicrobeType != MicrobeType.Purple && m.MicrobeType != MicrobeType.Pink && m.MicrobeType != MicrobeType.Blue).ToArray(),
                _ => microbes.Where(m => m.MicrobeType != MicrobeType.Pink || m.MicrobeType != MicrobeType.Red || m.MicrobeType != MicrobeType.Purple).ToArray()
            };

            return microbes.Length == 0 ? null : microbes.OrderBy(m => Vector3.Distance(seeker.transform.position, m.transform.position)).First();
        }

        public Microbe FindMate(Microbe seeker)
        {
            Microbe[] microbes = Agents.Where(a => a is Microbe m && m != seeker && m.IsAdult && m.State.GetType() == typeof(MicrobeSeekingMateState) && Vector3.Distance(seeker.transform.position, a.transform.position) < seeker.DetectionRange).Cast<Microbe>().ToArray();
            if (microbes.Length == 0)
            {
                return null;
            }
            
            microbes = seeker.MicrobeType switch
            {
                MicrobeType.Red => microbes.Where(m => m.MicrobeType == MicrobeType.Red || m.MicrobeType == MicrobeType.Orange || m.MicrobeType == MicrobeType.Pink).ToArray(),
                MicrobeType.Orange => microbes.Where(m => m.MicrobeType == MicrobeType.Orange || m.MicrobeType == MicrobeType.Yellow || m.MicrobeType == MicrobeType.Red).ToArray(),
                MicrobeType.Yellow => microbes.Where(m => m.MicrobeType == MicrobeType.Yellow || m.MicrobeType == MicrobeType.Green || m.MicrobeType == MicrobeType.Orange).ToArray(),
                MicrobeType.Green => microbes.Where(m => m.MicrobeType == MicrobeType.Green || m.MicrobeType == MicrobeType.Blue || m.MicrobeType == MicrobeType.Yellow).ToArray(),
                MicrobeType.Blue => microbes.Where(m => m.MicrobeType == MicrobeType.Blue || m.MicrobeType == MicrobeType.Purple || m.MicrobeType == MicrobeType.Green).ToArray(),
                MicrobeType.Purple => microbes.Where(m => m.MicrobeType == MicrobeType.Purple || m.MicrobeType == MicrobeType.Pink || m.MicrobeType == MicrobeType.Blue).ToArray(),
                _ => microbes.Where(m => m.MicrobeType == MicrobeType.Pink || m.MicrobeType == MicrobeType.Red || m.MicrobeType == MicrobeType.Purple).ToArray()
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
            floor.GetComponent<MeshRenderer>().material = floorMaterial;

            ResetAgents();
            
            for (int i = FindObjectsOfType<MicrobeBasePickup>().Length; i < activePickups; i++)
            {
                SpawnPickup();
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

                if (microbe.Hunger <= maxHunger && Vector3.Distance(Agents[index].transform.position, Vector3.zero) <= floorRadius && microbe.ElapsedLifespan < microbe.LifeSpan)
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

            for (int i = FindObjectsOfType<MicrobeBasePickup>().Length; i < activePickups; i++)
            {
                SpawnPickup();
            }
        }
        
        /// <summary>
        /// Render buttons to regenerate the floor or change its size..
        /// </summary>
        /// <param name="x">X rendering position. In most cases this should remain unchanged.</param>
        /// <param name="y">Y rendering position. Update this with every component added and return it.</param>
        /// <param name="w">Width of components. In most cases this should remain unchanged.</param>
        /// <param name="h">Height of components. In most cases this should remain unchanged.</param>
        /// <param name="p">Padding of components. In most cases this should remain unchanged.</param>
        /// <returns>The updated Y position after all custom rendering has been done.</returns>
        protected override float CustomRendering(float x, float y, float w, float h, float p)
        {
            // Regenerate the floor button.
            if (GuiButton(x, y, w, h, "Reset"))
            {
                ResetAgents();
            }
            
            return NextItem(y, h, p);
        }

        private void ResetAgents()
        {
            for (int i = Agents.Count - 1; i >= 0; i--)
            {
                Destroy(Agents[i].gameObject);
            }
            
            for (int i = 0; i < minMicrobes; i++)
            {
                SpawnMicrobe();
            }
        }

        private void SpawnMicrobe()
        {
            SpawnMicrobe((MicrobeType) Random.Range((int) MicrobeType.Red, (int) MicrobeType.Pink + 1));
        }

        private void SpawnMicrobe(MicrobeType microbeType)
        {
            Vector3 position = Random.insideUnitSphere * floorRadius;
            position = new Vector3(position.x, 0, position.z);
            
            SpawnMicrobe(microbeType, position);
        }

        private void SpawnMicrobe(MicrobeType microbeType, Vector3 position)
        {
            SpawnMicrobe(microbeType, position, Random.Range(minMicrobeSpeed, maxMicrobeSpeed), Random.Range(minMicrobeLifespan, maxMicrobeLifespan), Random.Range(minMicrobeDetectionRange, floorRadius * 2));
        }

        private void SpawnMicrobe(MicrobeType microbeType, Vector3 position, float moveSpeed, float lifespan, float detectionRange)
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
            microbe.DetectionRange = detectionRange;
            microbe.AssignMoveSpeed(moveSpeed);

            string n = microbeType switch
            {
                MicrobeType.Red => "Red",
                MicrobeType.Orange => "Orange",
                MicrobeType.Yellow => "Yellow",
                MicrobeType.Green => "Green",
                MicrobeType.Blue => "Blue",
                MicrobeType.Purple => "Purple",
                _ => "Pink"
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
            
            Instantiate(SpawnParticlesPrefab, microbe.transform.position, Quaternion.Euler(270, 0, 0));
        }

        private void SpawnPickup()
        {
            Vector3 position = Random.insideUnitSphere * floorRadius;
            position = new Vector3(position.x, 0, position.z);
            
            GameObject go = Instantiate(Random.Range(0, 4) switch
            {
                3 => fertilityPickupPrefab,
                2 => neverHungryPickupPrefab,
                1 => offspringPickupPrefab,
                _ => rejuvenatePickupPrefab
            }, position, Quaternion.identity);

            go.transform.localScale = new Vector3(0.5f, 1, 0.5f);
        }
    }
}