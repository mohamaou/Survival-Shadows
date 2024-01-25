using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Survivors
{
    public class SurvivorIcon : MonoBehaviour
    {
        [SerializeField] private Image playerIcon;
        [SerializeField] private GameObject scars, prison;


        
        public void InPrison(bool active)
        {
            prison.SetActive(active);
        }

 

        public void Crawling(bool active)
        {
            scars.SetActive(!prison.activeSelf && active);
        }
    }
}
