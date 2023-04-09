using Fusion;
using UnityEngine;

namespace FusionScripts
{
    public class Wall : NetworkBehaviour
    {
        public override void Spawned()
        {
            this.name = "[Network]Wall";
        }
        
        // void Start()
        // {
        //     if (HasStateAuthority)
        //     {
        //         this.startPoint = this.transform.position;
        //     }
        // }
        //
        // void Update()
        // {
        //     if (!HasStateAuthority)
        //     {
        //         return;
        //     }
        //     
        //     this.transform.position = this.startPoint;
        // }
        //
        // Vector3 startPoint;
    }
}