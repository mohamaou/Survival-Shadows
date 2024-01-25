using TMPro;
using UnityEngine;

namespace UI
{
    public class CosmeticsUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField playerName;




        public void ChangePlayerName()
        {
            PlayerPrefs.SetString("PlayerName",playerName.text);
        }
    }
}
