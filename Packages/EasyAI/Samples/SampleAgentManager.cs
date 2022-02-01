using UnityEngine;

namespace Samples
{
    public class SampleAgentManager : AgentManager
    {
        private enum AgentType : byte
        {
            Transform,
            Character,
            Rigidbody
        }
    
        protected override void Start()
        {
            CreateSampleArea(new Vector3(-15, 0, 0), AgentType.Transform);
            CreateSampleArea(Vector3.zero, AgentType.Character);
            CreateSampleArea(new Vector3(15, 0, 0), AgentType.Rigidbody);
            
            base.Start();
        }

        private static void CreateSampleArea(Vector3 position, AgentType agentType)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Quad);
            floor.name = $"{agentType} Agent Area";
            floor.transform.position = position;
            floor.transform.rotation = Quaternion.Euler(90, 0, 0);
            floor.transform.localScale = new Vector3(10, 10, 1);
        
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Quad);
            wall.name = "Wall Top";
            wall.transform.position = new Vector3(position.x, position.y + 2.5f, position.z + 5);
            wall.transform.localScale = new Vector3(10, 5, 1);
            wall.transform.SetParent(floor.transform);
        
            wall = GameObject.CreatePrimitive(PrimitiveType.Quad);
            wall.name = "Wall Bottom";
            wall.transform.position = new Vector3(position.x, position.y + 2.5f, position.z - 5);
            wall.transform.localRotation =  Quaternion.Euler(0, 180, 0);
            wall.transform.localScale = new Vector3(10, 5, 1);
            wall.transform.SetParent(floor.transform);
        
            wall = GameObject.CreatePrimitive(PrimitiveType.Quad);
            wall.name = "Wall Right";
            wall.transform.position = new Vector3(position.x + 5, position.y + 2.5f, position.z);
            wall.transform.localRotation =  Quaternion.Euler(0, 90, 0);
            wall.transform.localScale = new Vector3(10, 5, 1);
            wall.transform.SetParent(floor.transform);
        
            wall = GameObject.CreatePrimitive(PrimitiveType.Quad);
            wall.name = "Wall Left";
            wall.transform.position = new Vector3(position.x - 5, position.y + 2.5f, position.z);
            wall.transform.localRotation =  Quaternion.Euler(0, 270, 0);
            wall.transform.localScale = new Vector3(10, 5, 1);
            wall.transform.SetParent(floor.transform);

            GameObject agent = agentType switch
            {
                AgentType.Transform => EasyAIStatic.CreateTransformAgent(),
                AgentType.Character => EasyAIStatic.CreateCharacterAgent(),
                _ => EasyAIStatic.CreateRigidbodyAgent()
            };
            agent.transform.position = position;
            agent.transform.SetParent(floor.transform);
            agent.AddComponent<SampleMind>();
            SampleSensor sensor = agent.AddComponent<SampleSensor>();
            sensor.origin = position;
            sensor.size = 5;
            sensor.target = position;
            
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.transform.position = new Vector3(position.x + 2.5f, position.y + 2, position.z + 2.5f);
            Rigidbody rb = obstacle.AddComponent<Rigidbody>();
            rb.mass = 0.01f;
            
            obstacle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obstacle.transform.position = new Vector3(position.x + 2.5f, position.y + 2, position.z - 2.5f);
            rb = obstacle.AddComponent<Rigidbody>();
            rb.mass = 0.01f;
            
            obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.transform.position = new Vector3(position.x - 2.5f, position.y + 2, position.z - 2.5f);
            rb = obstacle.AddComponent<Rigidbody>();
            rb.mass = 0.01f;
            
            obstacle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obstacle.transform.position = new Vector3(position.x - 2.5f, position.y + 2, position.z + 2.5f);
            rb = obstacle.AddComponent<Rigidbody>();
            rb.mass = 0.01f;
        }
    }
}
