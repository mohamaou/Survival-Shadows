using UnityEngine;

namespace Game.Monster
{
    public class MonsterAnimationEvents : MonoBehaviour
    {
        [SerializeField] private NetworkMonster monster;
        [SerializeField] private MonsterAttacks monsterAttacks;

        public void EndAttack() => monster.AttackEnd();
        public void Attack() => monsterAttacks.Attack();
        public void HoldSurvivorClient() => monsterAttacks.HoldSurvivor();
    }
}
