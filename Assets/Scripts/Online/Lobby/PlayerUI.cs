using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Online.Lobby
{
    public class PlayerUI : MonoBehaviour 
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private GameObject gfx, vsIcon;
        [SerializeField] private GameObject monsterPanel, survivorPanel;


        private void Start()
        {
            Hide();
        }


        public void Hide()
        {
            if(gfx == null)return;
            gfx.SetActive(false);
            vsIcon.SetActive(false);
        }
        public void UpdatePlayer(Player player,bool active) 
        {
            try
            { 
                gfx.SetActive(active);
                playerNameText.text = player.Data[LobbyManager.KeyPlayerName].Value;
                var playerTyp = LobbyManager.GetPlayerTyp(player);
                vsIcon.SetActive(playerTyp == PlayerTyp.Monster && active);
                monsterPanel.SetActive(playerTyp == PlayerTyp.Monster);
                survivorPanel.SetActive(playerTyp == PlayerTyp.Survivor);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}