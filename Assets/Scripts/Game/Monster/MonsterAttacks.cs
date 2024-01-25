using Game.Survivors;
using Unity.Netcode;
using UnityEngine;

namespace Game.Monster
{
    public class MonsterAttacks : NetworkBehaviour
    {
        public static MonsterAttacks Instance { get; private set;}
        [SerializeField] private LayerMask survivorLayer;
        [SerializeField] [Range(0, 2)] private float range = 2f;
        [SerializeField] private Transform survivorPointHost, camPoint;
        [SerializeField] private MonsterSkin skin;
        [SerializeField] private new MonsterAnimationManager animation;


        private void Awake()
        {
            Instance = this;
        }

        public void Attack()
        {
            var s = GetNearSurvivor(SurvivorState.All);
            if(s != null && s.SurvivorState != SurvivorState.Crawling) s.SetSurvivorState(SurvivorState.Crawling);
        }

        public float GetRange() => range;

        public bool ShowHoldButton()
        {
            return GetNearSurvivor(SurvivorState.Crawling) != null;
        }
        public NetworkSurvivor GetNearSurvivor(SurvivorState state)
        {
            var result = new Collider[10];
            var size = Physics.OverlapSphereNonAlloc(transform.position, range, result, survivorLayer);
            if (size == 0) return null;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] != null)
                {
                    var s = result[i].GetComponent<NetworkSurvivor>();
                    if (s.SurvivorState == state || state == SurvivorState.All) return s;
                }
            }
            return null;
        }
        
        public void HoldSurvivor()
        {
            if (GetNearSurvivor(SurvivorState.Crawling) == null) return;
            animation.Hold(true);
            GetNearSurvivor(SurvivorState.Crawling).HoldMe(skin.HoldPoint());
        }

        public Transform GetHoldPoint() => !IsOwner ? skin.HoldPoint() : survivorPointHost;
        public Transform GetCam() => camPoint;
        
        private void OnDrawGizmos()
        { 
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,range);
        }
    }
}
