using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace A1
{
    public class FloorManager : MonoBehaviour
    {
        public static FloorManager Singleton;
        
        public readonly List<Floor> Floors = new List<Floor>();

        [SerializeField]
        private GameObject cleanerAgentPrefab;

        [SerializeField]
        private Vector2 floorSize = new Vector2(3, 1);

        [SerializeField]
        [Min(1)]
        private int floorScale = 1;

        [SerializeField]
        [Range(0, 1)]
        private float LikelyToGetDirtyChance;

        [SerializeField]
        [Min(0)]
        private float dirtTickRate = 1;

        [SerializeField]
        [Range(0, 1)]
        private float chanceDirty;

        [SerializeField]
        private Material materialCleanNormal;

        [SerializeField]
        private Material materialCleanLikelyToGetDirty;

        [SerializeField]
        private Material materialDirty;

        [SerializeField]
        private Material materialVeryDirty;

        [SerializeField]
        private Material materialExtremelyDirty;

        private GameObject _cleanerAgent;

        private float _elapsedTime;

        private void Awake()
        {
            if (Singleton == this)
            {
                return;
            }

            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            GenerateFloor();
        }

        private void GenerateFloor()
        {
            if (_cleanerAgent != null)
            {
                Destroy(_cleanerAgent.gameObject);
            }
            
            foreach (Floor floor in Floors)
            {
                Destroy(floor.gameObject);
            }
            Floors.Clear();
            
            Vector2 offsets = new Vector2((floorSize.x - 1) / 2f, (floorSize.y - 1) / 2f) * floorScale;
            for (int x = 0; x < floorSize.x; x++)
            {
                for (int y = 0; y < floorSize.y; y++)
                {
                    GenerateSection(new Vector2(x, y), offsets);
                }
            }

            _cleanerAgent = Instantiate(cleanerAgentPrefab, Vector3.zero, quaternion.identity);
            _cleanerAgent.name = "Cleaner Agent";
        }

        private void GenerateSection(Vector2 position, Vector2 offsets)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.transform.position = new Vector3(position.x * floorScale - offsets.x, 0, position.y * floorScale - offsets.y);
            go.transform.rotation = Quaternion.Euler(90, 0, 0);
            go.transform.localScale = new Vector3(floorScale, floorScale, 1);
            go.name = $"Floor {position.x} {position.y}";
            Destroy(go.GetComponent<Collider>());
            Floor floor = go.AddComponent<Floor>();
            bool likelyToGetDirty = Random.value < LikelyToGetDirtyChance;
            floor.Setup(likelyToGetDirty, likelyToGetDirty ? materialCleanLikelyToGetDirty : materialCleanNormal, materialDirty, materialVeryDirty, materialExtremelyDirty);
            Floors.Add(floor);
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime < dirtTickRate)
            {
                return;
            }

            _elapsedTime = 0;
            UpdateFloor();
        }

        private void UpdateFloor()
        {
            foreach (Floor floor in Floors.Where(f => f.State != Floor.DirtLevel.ExtremelyDirty))
            {
                float dirtChance = floor.LikelyToGetDirty ? chanceDirty * 2 : chanceDirty;
                
                for (int i = 0; i < 3; i++)
                {
                    if (Random.value <= dirtChance)
                    {
                        floor.Dirty();
                    }
                }
            }
        }
    }
}