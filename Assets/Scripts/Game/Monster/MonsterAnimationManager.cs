using System;
using UnityEngine;

namespace Game.Monster
{
    public class MonsterAnimationManager : MonoBehaviour
    {
        private static readonly int WalkKey = Animator.StringToHash("Walk");
        private static readonly int AttackKey = Animator.StringToHash("Attack");
        private static readonly int HorizontalKey = Animator.StringToHash("Horizontal");
        private static readonly int VerticalKey = Animator.StringToHash("Vertical");
        private static readonly int HoldingKey = Animator.StringToHash("Holding");
        private static readonly int OnHoldingKey = Animator.StringToHash("On Holding");
        private Vector2 direction;
        private bool walking, isHolding;
        [SerializeField] private Animator[] animations;


        public void SetWalk(bool walk)
        {
            walking = walk;
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i].SetBool(WalkKey,walk);
            }
        }
        public bool IsWalking() => walking;

        
        public void SetMoveDirection(Vector2 direction)
        {
            this.direction = Vector3.Lerp(this.direction, direction, 6 * Time.deltaTime);
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i].SetFloat(HorizontalKey,direction.y);
                animations[i].SetFloat(VerticalKey,direction.x);
            }
        }

        public Vector2 GetDirection() =>direction;
        
        
        public void Attack()
        {
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i].SetTrigger(AttackKey);
            }
        }
        

        public void Hold()
        {
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i].SetTrigger(OnHoldingKey);
            }
        }

        public bool IsHolding() => isHolding;
        public void Hold(bool hold)
        {
            isHolding = hold;
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i].SetBool(HoldingKey,hold);
            }
        }
    }
}
