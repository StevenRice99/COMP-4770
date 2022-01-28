using System.Collections.Generic;
using System.Linq;
using EasyAI.Managers;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace A1.Managers
{
    public class FloorManager : AgentManager
    {
        public static FloorManager FloorManagerSingleton => Singleton as FloorManager;
        
        public readonly List<Floor> Floors = new List<Floor>();

        [SerializeField]
        [Tooltip("The prefab for the cleaning agent that will be spawned in.")]
        private GameObject cleanerAgentPrefab;

        [SerializeField]
        [Tooltip("How many floor sections will be generated.")]
        private Vector2 floorSize = new Vector2(3, 1);

        [SerializeField]
        [Min(1)]
        [Tooltip("How many units wide will each floor section be generated as.")]
        private int floorScale = 1;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("The percentage chance that any floor section during generation will be likely to get dirty meaning the odds in increases in dirt level every time are double that of other floor sections.")]
        private float likelyToGetDirtyChance;

        [SerializeField]
        [Min(0)]
        [Tooltip("How many seconds between every time dirt is randomly added to the floor.")]
        private float timeBetweenDirtGeneration = 5;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("The percentage chance that a floor section will increase in dirt level during dirt generation.")]
        private float chanceDirty;

        [SerializeField]
        [Tooltip("The material applied to normal floor sections when they are clean.")]
        private Material materialCleanNormal;

        [SerializeField]
        [Tooltip("The material applied to like to get dirty floor sections when they are clean.")]
        private Material materialCleanLikelyToGetDirty;

        [SerializeField]
        [Tooltip("The material applied to a floor section when it is dirty.")]
        private Material materialDirty;

        [SerializeField]
        [Tooltip("The material applied to a floor section when it is very dirty.")]
        private Material materialVeryDirty;

        [SerializeField]
        [Tooltip("The material applied to a floor section when it is extremely dirty.")]
        private Material materialExtremelyDirty;

        private GameObject _cleanerAgent;

        private float _elapsedTime;

        protected override void Start()
        {
            base.Start();
            GenerateFloor();
        }

        protected override float CustomRendering(float x, float y, float w, float h, float p)
        {
            if (GuiButton(x, y, w, h, "Reset"))
            {
                GenerateFloor();
            }
            
            if (floorSize.x < 5)
            {
                y = NextItem(y, h, p);
                if (GuiButton(x, y, w, h, "Increase Size X"))
                {
                    floorSize.x++;
                    GenerateFloor();
                }
            }

            if (floorSize.x > 1)
            {
                y = NextItem(y, h, p);
                if (GuiButton(x, y, w, h, "Decrease Size X"))
                {
                    floorSize.x--;
                    GenerateFloor();
                }
            }
            
            if (floorSize.y < 5)
            {
                y = NextItem(y, h, p);
                if (GuiButton(x, y, w, h, "Increase Size Y"))
                {
                    floorSize.y++;
                    GenerateFloor();
                }
            }

            if (floorSize.y > 1)
            {
                y = NextItem(y, h, p);
                if (GuiButton(x, y, w, h, "Decrease Size Y"))
                {
                    floorSize.y--;
                    GenerateFloor();
                }
            }
            
            return NextItem(y, h, p);
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

            _elapsedTime = 0;
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
            bool likelyToGetDirty = Random.value < likelyToGetDirtyChance;
            floor.Setup(likelyToGetDirty, likelyToGetDirty ? materialCleanLikelyToGetDirty : materialCleanNormal, materialDirty, materialVeryDirty, materialExtremelyDirty);
            Floors.Add(floor);
        }

        protected override void Update()
        {
            base.Update();
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime < timeBetweenDirtGeneration)
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