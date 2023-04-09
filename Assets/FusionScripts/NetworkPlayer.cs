using Fusion;
using System;
using UnityEngine;

namespace FusionScripts
{
    public struct PlayerInformation : INetworkStruct
    {
        public int tokenHash;
        public Vector3 Position;
        public Quaternion Rotation;
    }

    public struct StarterAssetInputs : INetworkStruct
    {
        public Vector2 move;
        public Vector2 look;
        public NetworkBool jump;
        public NetworkBool sprint;
        public float cameraEulerY;
    }

    public class NetworkPlayer : NetworkBehaviour, IBeforeTick
    {
        [SerializeField] NetworkPlayerAgent agentPrefab;
        internal NetworkPlayerAgent AgentPrefab => this.agentPrefab;

        [Networked(OnChanged = nameof(OnActiveAgentChanged), OnChangedTargets = OnChangedTargets.All), HideInInspector]
        internal NetworkPlayerAgent ActiveAgent { get; private set; }
        [Networked] 
        internal ref PlayerInformation Information => ref MakeRef<PlayerInformation>();
        [Networked]
        internal ref StarterAssetInputs StarterAssetsInputs => ref MakeRef<StarterAssetInputs>();

        public static void OnActiveAgentChanged(Changed<NetworkPlayer> changed)
        {
            if (changed.Behaviour.ActiveAgent != null)
            {
                changed.Behaviour.AssignAgent(changed.Behaviour.ActiveAgent);
            }
            else
            {
                changed.Behaviour.ClearAgent();
            }
        }

        public void AssignAgent(NetworkPlayerAgent agent)
        {
            if (HasStateAuthority && this.ActiveAgent == agent)
            {
                return;
            }

            this.ActiveAgent = agent;
            this.ActiveAgent.Owner = this;

            Transform rendererAgent = agent.transform;
            if (HasInputAuthority)
            {
                rendererAgent = NetworkSceneContext.Instance.PlayerInput.agentTransform;
            }
            
            var renderers = rendererAgent.GetComponentsInChildren<Renderer>();
        }

        public void ClearAgent()
        {
            if (this.ActiveAgent == null)
            {
                return;
            }
            this.ActiveAgent.Owner = null;
            this.ActiveAgent = null;
        }

        public override void Spawned()
        {
            if(Runner.IsServer)
            {
                var token = Runner.GetPlayerConnectionToken(Object.InputAuthority);
                var guid = new Guid(token);
                this.Information.tokenHash = guid.GetHashCode();
            }
            this.name = "[Network]Player:" + this.Information.tokenHash;
            
            if (NetworkSceneContext.Instance.Game != null)
            {
                NetworkSceneContext.Instance.Game.Join(this);
            }
        }

        void IBeforeTick.BeforeTick()
        {
            if (GetInput(out PlayerInput input))
            {
                this.Information.Position = input.position;
                this.Information.Rotation = input.rotation;
                this.StarterAssetsInputs.move = input.move;
                this.StarterAssetsInputs.look = input.look;
                this.StarterAssetsInputs.jump = input.jump;
                this.StarterAssetsInputs.sprint = input.sprint;
                this.StarterAssetsInputs.cameraEulerY = input.cameraEulerY;
                
                bool agentValid = this.ActiveAgent != null && this.ActiveAgent.Object != null;
                if (agentValid)
                {
                    this.ActiveAgent.ApplyInput(input);
                }
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (!hasState)
            {
                return;
            }

            if (NetworkSceneContext.Instance.Game != null)
            {
                NetworkSceneContext.Instance.Game.Leave(this);
            }
            if (Object.HasStateAuthority && this.ActiveAgent != null)
            {
                Runner.Despawn(this.ActiveAgent.Object);
            }
            this.ActiveAgent = null;
        }
    }
}
