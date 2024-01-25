using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Online.Lobby
{
    public class PlayerIconHolder : MonoBehaviour
    {
        public static PlayerIconHolder Instance {get; protected set;}
        [SerializeField] private PlayerIcon[] survivorIcons;
        [SerializeField] private PlayerIcon monsterIcon;

        private void Awake()
        {
            Instance = this;
        }

        public void UpdatePlayersIcon(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            var players = lobby.Players;
            var survivors = new List<Player>();
            Player monster = null;
            for (int i = 0; i < players.Count; i++)
            {
                var playerTyp = LobbyManager.GetPlayerTyp(players[i]);
                if (playerTyp == PlayerTyp.Monster)
                {
                    monster = players[i];
                }
                else
                {
                    survivors.Add(players[i]);
                }
            }
            for (int i = 0; i < survivorIcons.Length; i++)
            {
                if (survivors.Count > i)
                {
                    survivorIcons[i].SetPlayer(survivors[i]);
                }
                else
                {
                    survivorIcons[i].SetPlayer(null);
                }
            }
            monsterIcon.SetPlayer(monster);
        }
    }
}
