using Fusion;
using UnityEngine;

namespace FusionScripts
{
    public class NetworkGame : NetworkBehaviour
    {
        [Networked, HideInInspector, Capacity(200)]
        public NetworkDictionary<PlayerRef, NetworkPlayer> Players { get; }

        public void Join(NetworkPlayer player)
        {
            var playerRef = player.Object.InputAuthority;
            if (!HasStateAuthority || this.Players.ContainsKey(playerRef))
            {
                return;
            }
            this.Players.Add(playerRef, player);
            this.SpawnPlayerAgent(player);
        }

        public void Leave(NetworkPlayer player)
        {
            if (!HasStateAuthority || !this.Players.ContainsKey(player.Object.InputAuthority))
            {
                return;
            }
            this.Players.Remove(player.Object.InputAuthority);
            this.DespawnPlayerAgent(player);
        }

        public override void Spawned()
        {
            this.name = "[Network]Game";
            NetworkSceneContext.Instance.Game = this;
            Runner.AddCallbacks(NetworkSceneContext.Instance.PlayerInput);
            if (!NetworkGame.spawnedGameObjects)
            {
                NetworkGame.spawnedGameObjects = true;
            }
            if (NetworkSceneContext.Instance.hostClientText != null)
            {
                NetworkSceneContext.Instance.hostClientText.text = Runner.IsServer ? "Host" : "Client";
            }
            Physics.IgnoreLayerCollision(7, 8, true);
        }

        void FixedUpdate()
        {
            if (NetworkSceneContext.Instance.countText != null)
            {
                NetworkSceneContext.Instance.countText.text = $"Count: {this.Players.Count}";
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            NetworkSceneContext.Instance.Game = null;
        }

        protected void SpawnPlayerAgent(NetworkPlayer player)
        {
            this.DespawnPlayerAgent(player);
            var agent = this.SpawnAgent(player.Object.InputAuthority, player.AgentPrefab, player.Information.Position, player.Information.Rotation);
            player.AssignAgent(agent);
        }

        protected void DespawnPlayerAgent(NetworkPlayer player)
        {
            if (null == player.ActiveAgent)
            {
                return;
            }
            this.DespawnAgent(player.ActiveAgent);
            player.ClearAgent();
        }

        NetworkPlayerAgent SpawnAgent(PlayerRef inputAuthority, NetworkPlayerAgent agentPrefab, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            var agent = Runner.Spawn(agentPrefab, spawnPosition, spawnRotation, inputAuthority);
            return agent;
        }

        void DespawnAgent(NetworkPlayerAgent agent)
        {
            if (null == agent)
            {
                return;
            }
            Runner.Despawn(agent.Object);
        }

        static bool spawnedGameObjects = false;
    }
}
