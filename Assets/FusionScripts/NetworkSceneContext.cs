using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FusionScripts
{
    public class NetworkSceneContext : MonoBehaviour
    {
        [SerializeField] internal NetworkPlayerInput PlayerInput;
        // [SerializeField] internal Text fpsText;
        [SerializeField] internal Text hostClientText;
        [SerializeField] internal Text countText;
        [SerializeField] internal TextMeshProUGUI keyCountText;

        internal static NetworkSceneContext Instance => NetworkSceneContext.instance;

        internal NetworkGame Game;

        void Awake()
        {
            NetworkSceneContext.instance = this;
        }
        static NetworkSceneContext instance;
    }
}