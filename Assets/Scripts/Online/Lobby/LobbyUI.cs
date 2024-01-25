using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;


namespace Online.Lobby
{
    public class LobbyUI : MonoBehaviour 
    {
        public static LobbyUI Instance {get; private set;}
        [SerializeField] private PlayerUI[] survivorsTemplate;
        [SerializeField] private PlayerUI monsterTemplate;
        [SerializeField] private GameObject playButton,backButton;
   


        private void Awake() 
        {
            Instance = this;
        }

        private void Start() 
        {
            LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
            LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
            LobbyManager.Instance.OnLobbyGameModeChanged += UpdateLobby_Event;
            LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
            LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnLeftLobby;
            Hide();
        }

        public void HideButtons()
        {
            playButton.SetActive(false);
            backButton.SetActive(false);
        }

        private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e) 
        {
            ClearLobby();
            Hide();
        }

        public void HideBackButton() => backButton.SetActive(false);
        private void Update()
        {
            playButton.SetActive(LobbyManager.Instance.IsLobbyHost());
        }

        private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e)
        {
            UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
        }

      

        private void UpdateLobby(Unity.Services.Lobbies.Models.Lobby lobby) 
        {
            if(lobby == null)return;
            ClearLobby();
            var survivors = new List<Player>();
            for (int i = 0; i < survivors.Count; i++)
            {
                survivorsTemplate[i].UpdatePlayer(survivors[i],true);
            }
            Show();
        }

        private void ClearLobby() 
        {
            monsterTemplate.Hide();
            for (int i = 0; i < survivorsTemplate.Length; i++)
            {
                survivorsTemplate[i].Hide();
            }
        }

        private void Hide() 
        {
            gameObject.SetActive(false);
        }

        private void Show()
        {
            try
            {
                gameObject.SetActive(true);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}