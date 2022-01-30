﻿using System;
using EasyAI.Agents;
using EasyAI.Managers;
using UnityEngine;

namespace EasyAI.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class LookAtAgentCamera : MonoBehaviour
    {
        /// <summary>
        /// The singleton look at agent camera.
        /// </summary>
        public static LookAtAgentCamera Singleton;

        [SerializeField]
        [Tooltip("How much to vertically offset the camera for viewing agents.")]
        private float offset;

        [SerializeField]
        [Min(0)]
        [Tooltip("How fast the camera should look to the agent for smooth looking. Set to zero for instant camera looking.")]
        private float lookSpeed;
        
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
        }

        private void Start()
        {
            float look = lookSpeed;
            lookSpeed = 0;
            LateUpdate();
            lookSpeed = look;
        }

        private void LateUpdate()
        {
            Agent agent = AgentManager.Singleton.SelectedAgent;
            if (agent == null)
            {
                agent = FindObjectOfType<Agent>();
            }
            
            if (agent == null)
            {
                return;
            }
            
            Vector3 target = agent.Visuals == null ? agent.Position : agent.Visuals.position;
            target = new Vector3(target.x, target.y + offset, target.z);

            if (lookSpeed <= 0)
            {
                transform.LookAt(target);
                return;
            }

            Transform t = transform;
            transform.rotation = Quaternion.Slerp(t.rotation, Quaternion.LookRotation(target - t.position), lookSpeed * Time.deltaTime);
        }
    }
}