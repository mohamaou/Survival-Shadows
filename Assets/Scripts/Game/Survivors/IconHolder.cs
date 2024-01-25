using System.Collections.Generic;
using UnityEngine;

namespace Game.Survivors
{
    public class IconHolder : MonoBehaviour
    {
        public static IconHolder Instance {get; private set;}
        [SerializeField] private List<SurvivorIcon> icons;

        private void Awake()
        {
            Instance = this;
            for (int i = 0; i < icons.Count; i++)
            {
                icons[i].gameObject.SetActive(false);
            }
        }

        public SurvivorIcon GetMyIcon()
        {
            var icon = icons[0];
            icon.gameObject.SetActive(true);
            icons.Remove(icons[0]);
            return icon;
        }
        
    }
}
