using System;
using Fusion;
using UnityEngine;

namespace FusionScripts
{
    public class Key : NetworkBehaviour
    {
        public override void Spawned()
        {
            this.name = "[Network]Key" + NetworkGame.keyCount;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                NetworkId keyId = new NetworkId();

                foreach (var k in NetworkGame.keys)
                {
                    if (this.Object.gameObject.name == k.gameObject.name)
                    {
                        keyId = k.Id;
                    }
                }

                if (NetworkGame.keys != null)
                {
                    NetworkGame.keys.Remove(NetworkGame.keys.Find(x => x.Id == keyId));
                }
                Runner.Despawn(this.Object);
            }
        }
    }
}