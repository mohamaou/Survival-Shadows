using UnityEngine;

namespace Game.Survivors
{
    public class SurvivorAnimation : MonoBehaviour
    {
        private static readonly int RunKey = Animator.StringToHash("Run");
        private static readonly int SpeedKey = Animator.StringToHash("Speed");
        private static readonly int MovementStateKey = Animator.StringToHash("Movement State");
        private static readonly int ReviveKey = Animator.StringToHash("Revive");
        private static readonly int HoldingKey = Animator.StringToHash("Holding");
        private static readonly int FixGeneratorKey = Animator.StringToHash("Fix");
        private static readonly int PutInThCageKey = Animator.StringToHash("Put In The Cage");
        private static readonly int ResetAnimationKey = Animator.StringToHash("Rest Animation");
        
        [SerializeField] private Animator[] anim;
        private float animationSpeed, movementState;
        private bool isWalking;
        
        
        
        public void Movement(bool walk, bool speed)
        {
            isWalking = walk;
            for (int i = 0; i < anim.Length; i++)
            { 
                anim[i].SetBool(RunKey,isWalking);
            }
            animationSpeed = Mathf.Lerp(animationSpeed, speed ? 1 : 0, 5 * Time.deltaTime);
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].SetFloat(SpeedKey ,animationSpeed);
            }
        }
        public void Movement(bool walk, float speed)
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].SetBool(RunKey,walk); 
                anim[i].SetFloat(SpeedKey ,speed);
            }
        }

        
        public (bool walk, float speed) GetMovementData()
        {
            return (isWalking, animationSpeed);
        }
        public void SetSurvivorState(SurvivorState state)
        {
            movementState = state switch
            {
                SurvivorState.Normal => 0,
                SurvivorState.Crouched => 0.5f,
                SurvivorState.Crawling => 1,
                _ => movementState
            };
        }

        public void Revive()
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].SetTrigger(ReviveKey);
            }
        }

        public void GetHold(bool h)
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].SetBool(HoldingKey,h);
            }
        }

        public void FixGenerator()
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].SetTrigger(FixGeneratorKey);
            }
        } 
        public void Idle()
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].SetTrigger(ResetAnimationKey);
            }
        }
        
        
        private void Update()
        {
            for (int i = 0; i < anim.Length; i++)
            { 
                var f = anim[i].GetFloat(MovementStateKey); 
                anim[i].SetFloat(MovementStateKey,Mathf.Lerp(f,movementState,6*Time.deltaTime));
            }
        }

        public void PutInCage()
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].SetTrigger(PutInThCageKey);
            }
        }
    }
}
