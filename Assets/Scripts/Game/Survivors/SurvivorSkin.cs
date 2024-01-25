using UnityEngine;

namespace Game.Survivors
{
   public class SurvivorSkin : MonoBehaviour
   {
      [SerializeField] private GameObject[] skins;
     
      

      public void SetSkin(int index)
      {
         for (int i = 0; i < skins.Length; i++)
         {
            skins[i].SetActive(index == i);
         }
      }
   }
}
