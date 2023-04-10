using Fusion;

namespace FusionScripts
{
    public class Wall : NetworkBehaviour
    {
        public override void Spawned()
        {
            this.name = "[Network]Wall";
        }
    }
}