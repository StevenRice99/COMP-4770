using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// Editor methods for creating EasyAI objects.
    /// </summary>
    public static class EditorFunctions
    {
        /// <summary>
        /// Create a transform agent.
        /// </summary>
        /// <param name="menuCommand">Automatically passed by Unity.</param>
        [MenuItem("GameObject/Easy AI/Agents/Transform Agent", false, 10)]
        private static void CreateTransformAgent(MenuCommand menuCommand)
        {
            GameObject agent = CreateAgent("Transform Agent");
            agent.AddComponent<TransformAgent>();
            FinishCreation(menuCommand, agent);
        }

        /// <summary>
        /// Create a character controller agent.
        /// </summary>
        /// <param name="menuCommand">Automatically passed by Unity.</param>
        [MenuItem("GameObject/Easy AI/Agents/Character Agent", false, 10)]
        private static void CreateCharacterAgent(MenuCommand menuCommand)
        {
            GameObject agent = CreateAgent("Character Agent");
            CharacterController c = agent.AddComponent<CharacterController>();
            c.center = new Vector3(0, 1, 0);
            agent.AddComponent<CharacterAgent>();
            FinishCreation(menuCommand, agent);
        }

        /// <summary>
        /// Create a rigidbody agent.
        /// </summary>
        /// <param name="menuCommand">Automatically passed by Unity.</param>
        [MenuItem("GameObject/Easy AI/Agents/Rigidbody Agent", false, 10)]
        private static void CreateRigidbodyAgent(MenuCommand menuCommand)
        {
            GameObject agent = CreateAgent("Rigidbody Agent");
            CapsuleCollider c = agent.AddComponent<CapsuleCollider>();
            c.center = new Vector3(0, 1, 0);
            Rigidbody rb = agent.AddComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.freezeRotation = true;
            agent.AddComponent<RigidbodyAgent>();
            FinishCreation(menuCommand, agent);
        }

        /// <summary>
        /// Create all types of cameras which only adds in those that are not yet present in the scene.
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/Easy AI/Cameras/All", false, 10)]
        private static void CreateAllCameras(MenuCommand menuCommand)
        {
            if (Object.FindObjectOfType<FollowAgentCamera>() == null)
            {
                CreateFollowAgentCamera(menuCommand);
            }
            
            if (Object.FindObjectOfType<LookAtAgentCamera>() == null)
            {
                CreateLookAtAgentCamera(menuCommand);
            }
            
            if (Object.FindObjectOfType<TrackAgentCamera>() == null)
            {
                CreateTrackAgentCamera(menuCommand);
            }
        }

        /// <summary>
        /// Create a follow agent camera.
        /// </summary>
        /// <param name="menuCommand">Automatically passed by Unity.</param>
        [MenuItem("GameObject/Easy AI/Cameras/Follow Camera", false, 10)]
        private static void CreateFollowAgentCamera(MenuCommand menuCommand)
        {
            if (Object.FindObjectOfType<FollowAgentCamera>() != null)
            {
                Debug.Log("Already have a follow agent camera in the scene.");
            }
            
            GameObject camera = CreateCamera("Follow Camera");
            camera.AddComponent<FollowAgentCamera>();
            FinishCreation(menuCommand, camera);
        }

        /// <summary>
        /// Create a look at agent camera.
        /// </summary>
        /// <param name="menuCommand">Automatically passed by Unity.</param>
        [MenuItem("GameObject/Easy AI/Cameras/Look At Camera", false, 10)]
        private static void CreateLookAtAgentCamera(MenuCommand menuCommand)
        {
            if (Object.FindObjectOfType<LookAtAgentCamera>() != null)
            {
                Debug.Log("Already have a look at agent camera in the scene.");
            }
            
            GameObject camera = CreateCamera("Look At Camera");
            camera.AddComponent<LookAtAgentCamera>();
            FinishCreation(menuCommand, camera);
        }

        /// <summary>
        /// Create a track agent camera.
        /// </summary>
        /// <param name="menuCommand">Automatically passed by Unity.</param>
        [MenuItem("GameObject/Easy AI/Cameras/Track Camera", false, 10)]
        private static void CreateTrackAgentCamera(MenuCommand menuCommand)
        {
            if (Object.FindObjectOfType<TrackAgentCamera>() != null)
            {
                Debug.Log("Already have a track agent camera in the scene.");
            }
            
            GameObject camera = CreateCamera("Track Camera");
            camera.AddComponent<TrackAgentCamera>();
            FinishCreation(menuCommand, camera);
        }

        /// <summary>
        /// Base method for setting up the core visuals of an agent.
        /// </summary>
        /// <param name="name">The name to give the agent.</param>
        /// <returns>Game object with the visuals setup for a basic agent.</returns>
        private static GameObject CreateAgent(string name)
        {
            GameObject agent = new GameObject(name);

            GameObject visuals = new GameObject("Visuals");
            visuals.transform.SetParent(agent.transform);
            visuals.transform.localPosition = Vector3.zero;
            visuals.transform.localRotation = Quaternion.identity;
            
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(visuals.transform);
            body.transform.localPosition = new Vector3(0, 1, 0);
            body.transform.localRotation = Quaternion.identity;
            Object.DestroyImmediate(body.GetComponent<CapsuleCollider>());
            
            GameObject eyes = GameObject.CreatePrimitive(PrimitiveType.Cube);
            eyes.name = "Eyes";
            eyes.transform.SetParent(body.transform);
            eyes.transform.localPosition = new Vector3(0, 0.4f, 0.25f);
            eyes.transform.localRotation = Quaternion.identity;
            eyes.transform.localScale = new Vector3(1, 0.2f, 0.5f);
            Object.DestroyImmediate(eyes.GetComponent<BoxCollider>());

            return agent;
        }

        /// <summary>
        /// Base method for setting up a camera.
        /// </summary>
        /// <param name="name">The name to give the camera.</param>
        /// <returns>Game object with a camera.</returns>
        private static GameObject CreateCamera(string name)
        {
            GameObject camera = new GameObject(name);
            camera.AddComponent<Camera>();
            return camera;
        }

        /// <summary>
        /// Finish adding a game object by nesting it under an object if it was selected.
        /// </summary>
        /// <param name="menuCommand">Automatically passed by Unity.</param>
        /// <param name="go">The game object that was created.</param>
        private static void FinishCreation(MenuCommand menuCommand, GameObject go)
        {
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}