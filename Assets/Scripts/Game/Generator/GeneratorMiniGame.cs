using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Generator
{
    public class GeneratorMiniGame : MonoBehaviour
    {
       public static GeneratorMiniGame Instance {get; private set;}
       [SerializeField] private Transform nail, targetAngle;
       [SerializeField] private Image slider,arcSlider;
       [SerializeField] private RectTransform sliderTransform;
       [SerializeField] private RectTransform sliderHandler;
       [SerializeField] private GameObject arc,sliderObject;
       [SerializeField] private float repairTime = 15f, thinkTime = 5f;
       private float randomTime,thinkTimeChanging;
       private System.Action repairFinish;
       

       private void Awake()
       {
           Instance = this;
       }

       private void Start()
       {
           arc.SetActive(false);
           sliderObject.SetActive(false);
       }

       private void RestCountDown()
       {
           StopAllCoroutines();
           StartCoroutine(CountDown());
       }
       private IEnumerator CountDown()
       {
           var repair = repairTime;
           randomTime = Random.Range(3f, 5f);
           thinkTimeChanging = thinkTime;
           arc.SetActive(false);
           sliderObject.SetActive(true);
           while (true)
           {
               repair -= Time.deltaTime;
              
               if(repair < 0)break;
               TapGame();
               SetSlider(1-(repair / repairTime));
               yield return null;
           }
           print("Finish");
           RepairFinish();
       }
       private void TapGame()
       {
           if (arc.activeSelf)
           {
               thinkTimeChanging -= Time.deltaTime;
               arcSlider.fillAmount = 1 - thinkTimeChanging / thinkTime;
               if (Input.GetMouseButtonDown(0))
               {
                   if (AngleDifference() > 6)
                   {
                       RestCountDown();
                   }
                   else
                   {
                       thinkTimeChanging = thinkTime;
                       arc.SetActive(false);
                       return;
                   }
               }
               if (thinkTimeChanging < 0) RestCountDown();
               return;
           }
           randomTime -= Time.deltaTime;
           if (!(randomTime < 0)) return;
           randomTime = Random.Range(3, 5);
           arc.SetActive(true);
           arc.transform.localScale = Vector3.zero;
           arc.transform.DOScale(Vector3.one, 0.2f);
       }
       private void SetSlider(float delta)
       {
           var parentWidth = sliderTransform.rect.width;
           var childWidth = sliderHandler.rect.width;
           var targetX = (parentWidth - childWidth) * (delta-0.5f);
           var newAnchoredPosition = new Vector2(targetX, sliderHandler.anchoredPosition.y);
           sliderHandler.anchoredPosition = newAnchoredPosition;
           slider.fillAmount = delta;
       }
       private float AngleDifference()
       {
           var nailAngle = nail.eulerAngles.z;
           var targetAngleValue = targetAngle.eulerAngles.z;
           var angleDifference = Mathf.DeltaAngle(nailAngle, targetAngleValue);
           return Mathf.Abs(angleDifference);
       }


       public void StartRepair(System.Action action)
       {
           repairFinish = action;
           StartCoroutine(CountDown());
       }

       public void Cancel()
       {
           StopAllCoroutines();
           arc.SetActive(false);
           sliderObject.SetActive(false);
       }
       private void RepairFinish()
       {
           arc.SetActive(false);
           sliderObject.SetActive(false);
           repairFinish?.Invoke();
       }
    }
}
