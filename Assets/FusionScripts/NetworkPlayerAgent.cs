using Fusion;
using StarterAssets;
using UnityEngine;

namespace FusionScripts
{
    public class NetworkPlayerAgent : NetworkBehaviour
    {
        public NetworkPlayer Owner
        {
            get { return this.networkPlayer; }
            set
            {
                this.networkPlayer = value;
                if(null != value)
                {
                    this.name = "[Network]PlayerAgent:" + this.networkPlayer.Information.tokenHash;
                }
            }
        }
        NetworkPlayer networkPlayer;

        public override void Spawned()
        {
            this.name = "[Network]PlayerAgent";
            this.starterAssetsInputs = this.GetComponent<StarterAssetsInputs>();
            
            if (this.mainCamera == null)
            {
                this.mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            
            if (HasInputAuthority)
            {
                var renderers = this.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.enabled = false;
                }
                if (this.TryGetComponent(out ThirdPersonController controller))
                {
                    controller.FootstepAudioVolume = 0;
                }
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            this.Owner = null;
        }

        public sealed override void FixedUpdateNetwork()
        {
            if (!IsProxy || this.starterAssetsInputs == null || this.Owner == null)
            {
                return;
            }
            
            Vector2 move = this.Owner.StarterAssetsInputs.move;
            if (null != this.mainCamera)
            {
                move = Quaternion.Euler(0, 0, -this.Owner.StarterAssetsInputs.cameraEulerY + this.mainCamera.transform.rotation.eulerAngles.y) * move;
            }
            
            this.starterAssetsInputs.MoveInput(move);
            this.starterAssetsInputs.LookInput(this.Owner.StarterAssetsInputs.look);
            this.starterAssetsInputs.JumpInput(this.Owner.StarterAssetsInputs.jump);
            this.starterAssetsInputs.SprintInput(this.Owner.StarterAssetsInputs.sprint);
        }

        internal void ApplyInput(PlayerInput input)
        {
            if (this.starterAssetsInputs == null || this.Owner == null)
            {
                return;
            }
            
            Vector2 move = input.move;
            if (this.mainCamera != null)
            {
                move = Quaternion.Euler(0, 0, -this.Owner.StarterAssetsInputs.cameraEulerY + this.mainCamera.transform.rotation.eulerAngles.y) * move;
            }
            
            this.starterAssetsInputs.MoveInput(move);
            this.starterAssetsInputs.LookInput(input.look);
            this.starterAssetsInputs.JumpInput(input.jump);
            this.starterAssetsInputs.SprintInput(input.sprint);
            
            if (0.5f < Vector3.Distance(this.transform.position, input.position))
            {
                this.transform.position = input.position;
            }
            this.transform.rotation = input.rotation;
        }

        GameObject mainCamera;
        StarterAssetsInputs starterAssetsInputs;
    }
}
