using UnityEngine;

namespace Start
{
    public class MonsterSkin : MonoBehaviour
    {
        [SerializeField] private GameObject[] skins;
       

        private void Start()
        {
            SetSkin(PlayerPrefs.GetInt("Monster Skin", 0));
        }

        public void SetSkin(int index)
        {
            for (int i = 0; i < skins.Length; i++)
            {
                skins[i].SetActive(index == i);
            }
        }
    }
}
