using System.Collections;
using UnityEngine;

namespace Start
{
    public class OptionCharacter : MonoBehaviour
    {
        [SerializeField] private Animator[] anim;
        [SerializeField] private Transform target;
        [SerializeField] private Renderer selectEffect;
        [SerializeField] private float effectOffset = 0.5f;
        private Vector3 startPoint;
        private bool activateSelectEffect = true;
        private Color effectColor= Color.white;
        private float passedTime;
        private static readonly int WalkForwardKey = Animator.StringToHash("Walk Forward");
        private static readonly int WalkBackwardKey = Animator.StringToHash("Walk Backward");

        private void Start()
        {
            startPoint = transform.position;
            selectEffect.gameObject.SetActive(false);
            StartCoroutine(SelectEffect());
        }
        

        private IEnumerator SelectEffect()
        {
            yield return new WaitForSeconds(effectOffset);
            selectEffect.gameObject.SetActive(true);
            while (activateSelectEffect)
            {
                passedTime += Time.deltaTime;
                var wave = Mathf.Sin(Mathf.PI * passedTime);
                wave = (wave / 2) + 0.5f;
                selectEffect.material.SetColor("_Color",effectColor *wave);
                yield return null;
            }
            selectEffect.gameObject.SetActive(false);
        }

        public void Move(System.Action optionSelected)
        {
            StartCoroutine(Walk(target.position,Vector3.forward, optionSelected));
            activateSelectEffect = false;
        }

        public void Back()
        {
            StartCoroutine(Walk(startPoint,Vector3.back,null));
            activateSelectEffect = false;
        }
        
        public void ResetPotion()
        {
            StartCoroutine(SelectEffect());
            transform.position = startPoint;
            activateSelectEffect = true;
        }

        private IEnumerator Walk(Vector3 targetPoint,Vector3 direction, System.Action reachDistance)
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].SetBool(direction == Vector3.forward ? WalkForwardKey: WalkBackwardKey,true);
            }
            while (Vector3.Distance(transform.position,targetPoint) > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPoint, 5 * Time.deltaTime);
                yield return null;
            }
            reachDistance?.Invoke();
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].SetBool(direction == Vector3.forward ? WalkForwardKey: WalkBackwardKey,false);
            }
        }
    }
}
