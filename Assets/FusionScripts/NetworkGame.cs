using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FusionScripts
{

    public class NetworkGame : NetworkBehaviour
    {
        [SerializeField] GameObject wall;
        [SerializeField] GameObject key;
        
        [Networked, HideInInspector, Capacity(200)]
        public NetworkDictionary<PlayerRef, NetworkPlayer> Players { get; }

        [Networked, HideInInspector] public NetworkString<_16> sharedKey { get; set; }

        public static int keyCount;

        private static int numRow = 9;
        private static int numCol = 9;
        private int[,,] map = new int[numRow, numCol, 5];
        
        private float scale = 5f;
        private float yPos = -1.75f;
        private float centre = 2.5f;

        public static List<NetworkObject> keys;

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
                keys = new List<NetworkObject>();
                this.GenerateMaze();
                keyCount = 0;
            }
            if (NetworkSceneContext.Instance.hostClientText != null)
            {
                NetworkSceneContext.Instance.hostClientText.text = Runner.IsServer ? "Host" : "Client";
            }
            // Physics.IgnoreLayerCollision(7, 8, true);
        }

        void FixedUpdate()
        {
            if (NetworkSceneContext.Instance.countText != null)
            {
                NetworkSceneContext.Instance.countText.text = $"Count: {this.Players.Count}";
            }

            if (NetworkSceneContext.Instance.keyCountText != null)
            {
                if (HasStateAuthority)
                {
                    sharedKey = "Keys\n" + (GameManager.doorCount - keys.Count);
                }
                NetworkSceneContext.Instance.keyCountText.text = sharedKey.ToString();
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

        void GenerateMaze()
        {
            if (!HasStateAuthority)
            {
                return;
            }
            
            int r = 0;
            int c = 4;
            (int R, int C) pos = (r, c);
            Stack<(int, int)> fringe = new Stack<(int, int)>();
            fringe.Push(pos);
            
            List<char> dir;
            char next;
            int index;
            
            while (fringe.Count != 0)
            {
                map[r, c, 4] = 1;
                dir = new List<char>();

                if (c > 0 && map[r, c - 1, 4] == 0)
                {
                    dir.Add('L');
                }
                if (r > 0 && map[r - 1, c, 4] == 0)
                {
                    dir.Add('U');
                }
                if (c < numCol - 1 && map[r, c + 1, 4] == 0)
                {
                    dir.Add('R');
                }
                if (r < numRow - 1 && map[r + 1, c, 4] == 0)
                {
                    dir.Add('D');
                }

                if (dir.Count != 0)
                {
                    pos = (r, c);
                    fringe.Push(pos);
                    index = Random.Range(0, dir.Count);
                    next = dir[index];

                    if (next == 'L')
                    {
                        map[r, c, 0] = 1;
                        c -= 1;
                        map[r, c, 2] = 1;
                    }
                    if (next == 'U')
                    {
                        map[r, c, 1] = 1;
                        r -= 1;
                        map[r, c, 3] = 1;
                    }
                    if (next == 'R')
                    {
                        map[r, c, 2] = 1;
                        c += 1;
                        map[r, c, 0] = 1;
                    }
                    if (next == 'D')
                    {
                        map[r, c, 3] = 1;
                        r += 1;
                        map[r, c, 1] = 1;
                    }
                }
                else
                {
                    pos = fringe.Pop();
                    r = pos.R;
                    c = pos.C;
                }
            }
            
            Vector3 spawnPos;

            for (int i = 0; i < numRow; i++)
            {
                for (int j = 0; j < numCol; j++)
                {
                    if (i < numRow - 1 && map[i, j, 3] == 0)
                    {
                        spawnPos = new Vector3(scale * (i + 1), yPos, scale * j + centre);
                        Runner.Spawn(this.wall, spawnPos, Quaternion.Euler(-90f, 90f, 0f));
                    }
                    if (j < numCol - 1 && map[i, j, 2] == 0)
                    {
                        spawnPos = new Vector3(scale * i + centre, yPos, scale * (j + 1));
                        Runner.Spawn(this.wall, spawnPos, Quaternion.Euler(-90f, 0f, 0f));
                    }
                }
            }
            
            List<Vector3> spawnPosVal = new List<Vector3>();

            for (int i = 0; i < 3; i++)
            {
                r = Random.Range(0, numRow - 1);
                c = Random.Range(0, numCol - 1);
                do
                {
                    spawnPos = new Vector3(scale * r + centre, 0, scale * c + centre);
                } while (spawnPosVal.Contains(spawnPos));
                spawnPosVal.Add(spawnPos);
                keyCount++;
                keys.Add(Runner.Spawn(this.key, spawnPos, Quaternion.Euler(-90f, 45f, 0f)));
            }
        }
        static bool spawnedGameObjects = false;
    }
}
