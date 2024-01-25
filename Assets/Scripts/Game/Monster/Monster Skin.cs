using UnityEngine;

namespace Game.Monster
{
    public class MonsterSkin : MonoBehaviour
    {
        [SerializeField] private GameObject[] thirdPersonSkins, firstPersonSkins;
        [SerializeField] private Transform[] holdPoints;
        private int _skinIndex;
        
        
        public void SetSkin(int index)
        {
            _skinIndex = index;
            for (int i = 0; i < thirdPersonSkins.Length; i++)
            {
                thirdPersonSkins[i].SetActive(index == i);
                firstPersonSkins[i].SetActive(index == i);
            }
        }
        public Transform HoldPoint() => holdPoints[_skinIndex];
    }
}
