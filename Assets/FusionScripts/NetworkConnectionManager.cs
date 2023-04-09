using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FusionScripts
{
    [DisallowMultipleComponent]
    public class NetworkConnectionManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] bool connectOnAwake = true;
        [SerializeField] NetworkRunner networkRunnerPrefab;
        [SerializeField] string sessionName = "maze";

        void Awake()
        {
            if (this.connectOnAwake)
            {
                StartCoroutine(this.CoInstantiateNetworkRunner());
            }
        }

        IEnumerator CoInstantiateNetworkRunner()
        {
            if (!this.networkRunnerPrefab)
            {
                yield break;
            }
            NetworkRunner runner = Instantiate(this.networkRunnerPrefab);
            runner.AddCallbacks(this);
            DontDestroyOnLoad(runner);
            runner.name = "[Network]Runner";

            if (this.gameObject.transform.parent)
            {
                this.gameObject.transform.parent = null;
            }
            DontDestroyOnLoad(this.gameObject);
            var clientTask = this.InitializeNetworkRunner(runner);
        }

        Task InitializeNetworkRunner(NetworkRunner runner)
        {
            var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
            if (null == sceneManager)
            {
                sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }
            
            this.connectionToken = Guid.NewGuid().ToByteArray();
            return runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.AutoHostOrClient,
                SessionName = this.sessionName,
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                ConnectionToken = this.connectionToken,
            });
        }

        async void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            await runner.Shutdown(true, ShutdownReason.HostMigration);

            runner = Instantiate(this.networkRunnerPrefab);
            runner.AddCallbacks(this);
            DontDestroyOnLoad(runner);
            runner.name = "[Network]Runner:" + hostMigrationToken.GameMode;
            
            var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
            if (null == sceneManager)
            {
                sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }
            
            await runner.StartGame(new StartGameArgs
            {
                HostMigrationToken = hostMigrationToken,
                HostMigrationResume = HostMigrationResume,
                SceneManager = sceneManager,
                ConnectionToken = this.connectionToken,
            });
        }

        void HostMigrationResume(NetworkRunner runner)
        {
            foreach (var resumeNO in runner.GetResumeSnapshotNetworkObjects())
            {
                if (resumeNO.TryGetBehaviour<NetworkPositionRotation>(out var posRot))
                {
                    runner.Spawn(resumeNO, position: posRot.ReadPosition(), rotation: posRot.ReadRotation(), onBeforeSpawned: (runner, newNO) =>
                    {
                        newNO.CopyStateFrom(resumeNO);
                        if (resumeNO.TryGetBehaviour<NetworkBehaviour>(out var myCustomNetworkBehaviour))
                        {
                            newNO.GetComponent<NetworkBehaviour>().CopyStateFrom(myCustomNetworkBehaviour);
                        }
                    });
                }
            }
        }

        #region INetworkRunnerCallbacks
        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }

        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }
        #endregion

        byte[] connectionToken;
    }
}