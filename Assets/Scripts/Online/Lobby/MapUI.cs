using Game.General;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Online.Lobby
{
    public class MapUI : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        [SerializeField] private int mapIndex, cost;
        [SerializeField] private GameObject lockObject, selectText, playOptionPanel;
        [SerializeField] private Image mapImage;
        [SerializeField] private TextMeshProUGUI costText;

        private void Start()
        {
            //playOptionPanel.SetActive(false);
            if (IHaveTheMap())
            {
                costText.gameObject.SetActive(false);
                selectText.SetActive(true);
                mapImage.color = Color.white;
                lockObject.SetActive(false);
            }
            else
            {
                costText.text = cost.ToString();
                costText.color = Coins.IsCoinEnough(cost) ? Color.green : Color.red;
                selectText.SetActive(false);
                costText.gameObject.SetActive(true);
                mapImage.color = Color.gray;
                lockObject.SetActive(true);
            }
        }

        private bool IHaveTheMap()
        {
            return PlayerPrefs.GetInt("I Have This Map" + mapIndex, mapIndex == 1 ? 1 : 0) == 1;
        }
        public void PressButton()
        {
            if (IHaveTheMap())
            {
                PlayerPrefs.SetString("Selected Map", sceneName); 
                playOptionPanel.SetActive(true);
            }
            else
            {
                if(!Coins.IsCoinEnough(cost,true))return;
                PlayerPrefs.SetInt("I Have This Map" + mapIndex, 1);
                costText.gameObject.SetActive(false);
                selectText.SetActive(true);
                mapImage.color = Color.white;
                lockObject.SetActive(false);
            }
        }
    }
}
