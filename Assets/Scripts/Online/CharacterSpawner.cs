using System.Collections;
using System.Collections.Generic;
using Online.Lobby;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Online
{
    public class CharacterSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject survivor,monster;
        [SerializeField] private List<Transform> survivorSpawnPoint;
        [SerializeField] private Transform monsterSpawnPoint;
        private readonly List<NetworkObject> players = new List<NetworkObject>();
        


        private void Start()
        {
            if (NetworkManager.Singleton != null)
            { 
                SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId,AuthenticationService.Instance.PlayerId);
                StartCoroutine(AllThePlayerOnTheScene());
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }

        #region Create Player
        private PlayerTyp GetPlayerTyp(string authenticationId)
        {
            return LobbyManager.Instance.GetPlayerTyp(authenticationId);
        }
        [ServerRpc (RequireOwnership = false)]
        private void SpawnPlayerServerRpc(ulong clientId,string authenticationId)
        {
            var playerTyp = GetPlayerTyp(authenticationId); 
            var player = playerTyp == PlayerTyp.Monster ? monster : survivor;
            var netObj = Instantiate(player, GetSpawnPoint(playerTyp), Quaternion.identity);
            netObj.SpawnAsPlayerObject(clientId,true);
            netObj.transform.name = "Player " + clientId;
            players.Add(netObj);
        }
        private Vector3 GetSpawnPoint(PlayerTyp playerTyp)
        {
            var point =playerTyp == PlayerTyp.Monster ? monsterSpawnPoint.position : survivorSpawnPoint[0].position;
            if (playerTyp == PlayerTyp.Survivor) survivorSpawnPoint.Remove(survivorSpawnPoint[0]);
            return point;
        }
        #endregion

        private IEnumerator AllThePlayerOnTheScene()
        {
            while (LobbyManager.PlayerCount > players.Count)
            {
                yield return null;
            }
            transform.name = "Done";
        }
    }
}
