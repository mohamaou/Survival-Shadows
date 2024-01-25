using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Online.Relay;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
public enum PlayerTyp
{
    Monster,
    Survivor
}
namespace Online.Lobby
{
    public class LobbyManager : MonoBehaviour 
    {
        public static LobbyManager Instance {get; private set;}
        
        public const string KeyPlayerName = "PlayerName";
        public const string KeyPlayerSurvivorSkin = "PlayerSurvivorSkin";
        public const string KeyPlayerHat = "PlayerHat";
        public const string KeyPlayerBag = "PlayerBag";
        public const string KeyPlayerMonsterSkin = "PlayerMonsterSkin";
        public const string KeyPlayerTyp = "CharacterTyp";
        private const string KeyHasAMonster = "HasMonster";
        private const string KeySurvivorFull = "FullOfSurvivor";
        private const string KeyMap = "MapName";
        private const string KeyPrivateData = "PrivateData";
        private const string KeyRelayCode = "RelayCode";

        
        public event EventHandler OnLeftLobby;
        public event EventHandler<LobbyEventArgs> OnJoinedLobby;
        public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
        public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
        public event EventHandler<LobbyEventArgs> OnLobbyGameModeChanged;
        public class LobbyEventArgs : EventArgs {
            public Unity.Services.Lobbies.Models.Lobby lobby;
        }
        public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
        public class OnLobbyListChangedEventArgs : EventArgs 
        {
            public List<Unity.Services.Lobbies.Models.Lobby> lobbyList;
        }
        
        
        
        [SerializeField] private int maxPlayerCount = 5;
        private PlayerTyp localPlayerTyp;
        private float heartbeatTimer;
        private float lobbyPollTimer;
        private float refreshLobbyListTimer = 5f;
        private Unity.Services.Lobbies.Models.Lobby joinedLobby;
        public static int PlayerCount;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                PlayerCount = 0;
                Authenticate(Random.Range(0,1500).ToString());
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            RelayManager.ResetRelyCode();
            NetworkManager.Singleton.OnClientConnectedCallback += LogIn;
        }

        private void LogIn(ulong id)
        {
            print("Player Loged In With Id: " +id);
        }

        public int GetMaxPlayerCount() => maxPlayerCount;
        
        
        
        private void Update() 
        {
            HandleRefreshLobbyList(); 
            HandleLobbyHeartbeat();
            HandleLobbyPolling();
        }

        public async void Authenticate(string playerName) 
        {
            var initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(playerName);

            await UnityServices.InitializeAsync(initializationOptions);

            AuthenticationService.Instance.SignedIn += () => 
            {
                // do nothing
                Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);
                RefreshLobbyList();
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private void HandleRefreshLobbyList() 
        {
            if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn) 
            {
                refreshLobbyListTimer -= Time.deltaTime;
                if (refreshLobbyListTimer < 0f) {
                    float refreshLobbyListTimerMax = 5f;
                    refreshLobbyListTimer = refreshLobbyListTimerMax;

                    RefreshLobbyList();
                }
            }
        }

        private async void HandleLobbyHeartbeat()
        {
            if (!IsLobbyHost()) return;
            heartbeatTimer -= Time.deltaTime;
            if (!(heartbeatTimer < 0f)) return;
            var heartbeatTimerMax = 15f;
            heartbeatTimer = heartbeatTimerMax;
            await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
        }

       

        public Unity.Services.Lobbies.Models.Lobby GetJoinedLobby() 
        {
            if(joinedLobby == null)LeaveLobby();
            return joinedLobby;
        }
      

        
      



        #region Player Data
        public static PlayerTyp GetPlayerTyp(Player player)
        {
            return player.Data[LobbyManager.KeyPlayerTyp].Value switch
            {
                "Monster" => PlayerTyp.Monster,
                _ => PlayerTyp.Survivor
            };
        }
        private Player GetPlayerData()
        {
            var playerName = PlayerPrefs.GetString("PlayerName", "player");
            var monsterSkin = PlayerPrefs.GetInt("Monster Skin", 0).ToString();
            var survivorSkin = PlayerPrefs.GetInt("Survivor Skin", 0).ToString();
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>()
                {
                    {KeyPlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,playerName)},
                    {KeyPlayerTyp, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,localPlayerTyp.ToString())},
                    {KeyPlayerSurvivorSkin, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,survivorSkin)},
                    {KeyPlayerMonsterSkin, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, monsterSkin)},
                }
            };
        }

        public PlayerTyp GetPlayerTyp(string authenticationId)
        {
            foreach (var player in joinedLobby.Players)
            {
                if (player.Id == authenticationId) return GetPlayerTyp(player);
            }
            return PlayerTyp.Survivor;
        }
        private bool IsPlayerInLobby() 
        {
            if (joinedLobby != null && joinedLobby.Players != null) {
                foreach (Player player in joinedLobby.Players) {
                    if (player.Id == AuthenticationService.Instance.PlayerId) {
                        // This player is in this lobby
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsLobbyHost() 
        { 
            if (joinedLobby == null) return false;
            return AuthenticationService.Instance.PlayerId ==  joinedLobby.HostId;
        }
        public void SetPlayerTyp(PlayerTyp playerTyp)
        {
            localPlayerTyp = playerTyp;
        }

        private string HasAMonster(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            for (int i = 0; i < lobby.Players.Count; i++)
            {
                if (lobby.Players[i].Data[KeyPlayerTyp].Value == PlayerTyp.Monster.ToString()) return "1";
            }
            return "0";
        }

        private string FullOfSurvivor(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            var playersCount = 0;
            for (int i = 0; i < lobby.Players.Count; i++)
            {
                if (lobby.Players[i].Data[KeyPlayerTyp].Value == PlayerTyp.Survivor.ToString()) playersCount++;
            }

            return playersCount == 4 ? "1" : "0";
        }
        #endregion

        #region Creat Join Lobby
        public async void CreateLobby(string lobbyName, int maxPlayers, bool privateLobby = false) 
        {
            var player = GetPlayerData();
            var mapName = PlayerPrefs.GetString("Selected Map");
            var hasAMonster = localPlayerTyp == PlayerTyp.Monster ? "1" : "0"; 
            var options = new CreateLobbyOptions
            {
                Player = player,
                IsPrivate = privateLobby,
                Data = new Dictionary<string, DataObject>
                {
                    { KeyHasAMonster, new DataObject(DataObject.VisibilityOptions.Public, hasAMonster,DataObject.IndexOptions.S1) },
                    { KeySurvivorFull, new DataObject(DataObject.VisibilityOptions.Public,"0" ,DataObject.IndexOptions.S2) },
                    { KeyMap , new DataObject(DataObject.VisibilityOptions.Public,mapName,DataObject.IndexOptions.S3)},
                    { KeyPrivateData , new DataObject(DataObject.VisibilityOptions.Member,"Only Member Can See This")}
                }
            };

            var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            joinedLobby = lobby;
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
            LobbyJoined();
            Debug.Log("Created Lobby " + lobby.Name);
        }
        public async void QuickJoinLobby(string lobbyName, int maxPlayers) 
        {
            try 
            {
                var options =  new QuickJoinLobbyOptions
                {
                    Player = GetPlayerData(),
                    Filter = localPlayerTyp == PlayerTyp.Monster?
                        new List<QueryFilter>() 
                        { 
                            new (field: QueryFilter.FieldOptions.S1, op: QueryFilter.OpOptions.EQ, value: "0"), 
                            new (field: QueryFilter.FieldOptions.S3, op: QueryFilter.OpOptions.EQ, value: PlayerPrefs.GetString("Selected Map"))
                        }:
                        new List<QueryFilter>() 
                        { 
                            new (field: QueryFilter.FieldOptions.S2, op: QueryFilter.OpOptions.EQ, value: "0") ,
                            new (field: QueryFilter.FieldOptions.S3, op: QueryFilter.OpOptions.EQ, value: PlayerPrefs.GetString("Selected Map"))
                        }
                };
                var lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
                joinedLobby = lobby;
                OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
                LobbyJoined();
                print("Joined Lobby With ID: "+ joinedLobby.Id);
            } 
            catch (LobbyServiceException e) 
            {
                CreateLobby(lobbyName,maxPlayers,false);
                Debug.Log(e);
            }
        }
        public async void JoinLobbyByCode(string lobbyCode) 
        {
            try
            {
                var player = GetPlayerData();
                var lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions { 
                    Player = player
                });

                joinedLobby = lobby;

                OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
                LobbyJoined();
            }
            catch (Exception e)
            { 
                PlayOption.Instance.NoLobbyFound();
            }
            
        }
        public async void JoinLobby(Unity.Services.Lobbies.Models.Lobby lobby) 
        {
            Player player = GetPlayerData();

            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions {
                Player = player
            });

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
            LobbyJoined();
        }
        #endregion
        
        #region Update Lobby
        public async void RefreshLobbyList() 
        {
            try 
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions();
                options.Count = 25;

                // Filter for open lobbies only
                options.Filters = new List<QueryFilter> {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0")
                };

                // Order by newest lobbies first
                options.Order = new List<QueryOrder> {
                    new QueryOrder(
                        asc: false,
                        field: QueryOrder.FieldOptions.Created)
                };

                QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

                OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
        public async void UpdatePlayerName(string playerName) 
        {
            if (joinedLobby != null) 
            {
                try {
                    UpdatePlayerOptions options = new UpdatePlayerOptions();

                    options.Data = new Dictionary<string, PlayerDataObject>() {
                        {
                            KeyPlayerName, new PlayerDataObject(
                                visibility: PlayerDataObject.VisibilityOptions.Public,
                                value: playerName)
                        }
                    };

                    string playerId = AuthenticationService.Instance.PlayerId;

                    Unity.Services.Lobbies.Models.Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                    joinedLobby = lobby;

                    OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                    LobbyJoined();
                } catch (LobbyServiceException e) {
                    Debug.Log(e);
                }
            }
        }
        public async void UpdatePlayerCharacter(PlayerTyp playerTyp) 
        {
            if (joinedLobby != null) 
            {
                try {
                    UpdatePlayerOptions options = new UpdatePlayerOptions();
                    options.Data = new Dictionary<string, PlayerDataObject>() 
                    {
                        {
                            KeyHasAMonster, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,HasAMonster(joinedLobby))
                        } 
                    };
                    var playerId = AuthenticationService.Instance.PlayerId;
                    var lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                    joinedLobby = lobby;

                    OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                    LobbyJoined();
                } catch (LobbyServiceException e) {
                    Debug.Log(e);
                }
            }
        }
        private async void HandleLobbyPolling()
        {
            if (joinedLobby == null) return;
            lobbyPollTimer -= Time.deltaTime;
            if (!(lobbyPollTimer < 0f)) return;
            var lobbyPollTimerMax = 1.1f;
            if (!joinedLobby.Data.TryGetValue(KeyPrivateData, out var p)) LeaveLobby(true);
            lobbyPollTimer = lobbyPollTimerMax;
            try
            {
                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                if (PlayerIconHolder.Instance != null) PlayerIconHolder.Instance.UpdatePlayersIcon(joinedLobby);
                OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            if (!IsLobbyHost())
            {
                try
                {
                    if (joinedLobby.Data.TryGetValue(KeyRelayCode, out var value))
                    { 
                        await RelayManager.JoinRelay(value.Value);
                    }
                }
                catch (RelayServiceException e)
                {
                    Debug.LogError(e);
                    throw;
                }
            }
            else
            {
                try
                {
                    var option = new UpdateLobbyOptions
                    {
                        Data = new Dictionary<string, DataObject>
                        {
                            { 
                                KeySurvivorFull ,new DataObject(DataObject.VisibilityOptions.Public,FullOfSurvivor(joinedLobby),DataObject.IndexOptions.S2)
                            },
                            { 
                                KeyHasAMonster, new DataObject(DataObject.VisibilityOptions.Public, HasAMonster(joinedLobby),DataObject.IndexOptions.S1)
                            }
                        }
                    };
                    await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, option);
                    PlayerCount = joinedLobby.Players.Count;
                    if (PlayerCount == 5)
                    {
                        NetworkManager.Singleton.SceneManager.LoadScene("Map 1",LoadSceneMode.Single);
                        Destroy(gameObject);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }
            }
        }
        private async Task UpdateRelayCode(string relayCode)
        {
            try
            {
                var options = new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            KeyRelayCode, new DataObject (visibility: DataObject.VisibilityOptions.Member,relayCode)
                        }
                    }
                };
                joinedLobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, options);
            }
            catch (Exception e)
            {
                print(e);
                throw;
            }
        }
        #endregion
        
        public async void Play()
        {
            if (SceneManager.GetActiveScene().buildIndex != 0 || !IsLobbyHost()) return;
            try
            { 
                var relayCode = await RelayManager.CreateRelay();
                await UpdateRelayCode(relayCode);
                NetworkManager.Singleton.SceneManager.LoadScene("Lobby Room", LoadSceneMode.Single);
            }
            catch (Exception e)
            { 
                Debug.Log(e);
            }
        }

        
        #region Leave Or Join
        private void OnApplicationQuit()
        {
            LeaveLobby();
        }
        private void LobbyJoined()
        {
            if(IsLobbyHost())Play();
        }
        public async void LeaveLobby(bool gameEnd = false) 
        {
            if (joinedLobby != null)
            {
                try 
                {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                    joinedLobby = null;
                    PlayerCount = 0;
                    if (IsLobbyHost()) RelayManager.Logout();
                    OnLeftLobby?.Invoke(this, EventArgs.Empty);
                } 
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
            if(gameEnd) RelayManager.Logout();
            RelayManager.ResetRelyCode();
            joinedLobby = null;
            if (SceneManager.GetActiveScene().buildIndex != 0) SceneManager.LoadScene(0);
        }
   
        public async void KickPlayer(string playerId)
        {
            if (IsLobbyHost()) 
            {
                try 
                { 
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
                } 
                catch (LobbyServiceException e) 
                {
                    Debug.Log(e);
                }
            }
        }
        #endregion
    }
}