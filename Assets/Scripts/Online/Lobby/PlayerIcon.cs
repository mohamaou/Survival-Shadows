using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Online.Lobby
{
    public class PlayerIcon : MonoBehaviour
    {
        [SerializeField] private PlayerTyp playerTyp;
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private GameObject lickButton, loading;
        [SerializeField] private Image playerImage;
        private string _myId;
        [SerializeField] private Sprite[] survivorSkins, monsterSkins;

        private void Start()
        {
            loading.SetActive(true);
            playerName.gameObject.SetActive(false);
            playerImage.gameObject.SetActive(false);
            lickButton.SetActive(false);
        }
        
        
        public void SetPlayer(Player player) 
        {
            if (player == null)
            {
                playerName.gameObject.SetActive(false);
                playerImage.gameObject.SetActive(false);
                lickButton.SetActive(false);
                loading.SetActive(true);
            }
            else
            {
                try
                {
                    playerImage.gameObject.SetActive(true);
                    playerName.gameObject.SetActive(true);
                    loading.SetActive(false);
                    playerName.text = player.Data[LobbyManager.KeyPlayerName].Value;
                    lickButton.SetActive(LobbyManager.Instance.IsLobbyHost() && AuthenticationService.Instance.PlayerId != player.Id);
                    var skinKey = playerTyp == PlayerTyp.Monster ? LobbyManager.KeyPlayerMonsterSkin : LobbyManager.KeyPlayerSurvivorSkin;
                    var skinIndex = int.Parse(player.Data[skinKey].Value);
                    playerImage.sprite = playerTyp == PlayerTyp.Monster ? monsterSkins[skinIndex] : survivorSkins[skinIndex];
                    _myId = player.Id;
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }

        public void KickPlayer()
        {
            LobbyManager.Instance.KickPlayer(_myId);
        }
        
    }
}
