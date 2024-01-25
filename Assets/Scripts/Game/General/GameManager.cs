using System;
using System.Collections;
using System.Collections.Generic;
using Game.Monster;
using Game.Survivors;
using Game.UI;
using Unity.Netcode;
using UnityEngine;

public enum GameState
{
    Loading, Start, Play, Lose, Win
}

public enum EndGameTyp
{
    TimeOut, AllTheSurvivorsRuns, AllSurvivorInCage
}

public class GameManager : NetworkBehaviour
{
    public bool waitRoom;
    public static GameManager Instance {get; protected set;}
    public static GameState State;
    private NetworkMonster myMonster;
    private NetworkSurvivor mySurvivor;
    private List<NetworkSurvivor> survivors = new List<NetworkSurvivor>();
    
    
    private void Awake()
    {
        Instance = this;
        State = GameState.Loading;
    }
    private void Start()
    {
        StartCoroutine(CheckPlayersState());
    }


    #region SetPlayers

    public void SetOtherPlayers()
    {
        foreach (var s in FindObjectsOfType<NetworkSurvivor>())
        {
            s.Play();
            survivors.Add(s);
        }
    }
    public void SetMyMonster(NetworkMonster monster) => myMonster = monster;
    public void SetMySurvivor(NetworkSurvivor survivor) => mySurvivor = survivor;
    public NetworkMonster GetMonster() => myMonster;
    public NetworkSurvivor GetSurvivor() => mySurvivor;
    #endregion

    private IEnumerator CheckPlayersState()
    {
        yield return new WaitUntil(() => State == GameState.Play);
        while (State == GameState.Play)
        {
            var allPlayerOut = true;
            var allSurvivorsInCages = true;
            for (int i = 0; i < survivors.Count; i++)
            {
                if (survivors[i].GetState() != SurvivorState.Out) allPlayerOut = false;
                if (!survivors[i].IsInCage()) allSurvivorsInCages = false;
            }

            if (survivors.Count > 0)
            {
                if (allPlayerOut)
                {
                    EndGame(EndGameTyp.AllTheSurvivorsRuns);
                    break;
                }

                if (allSurvivorsInCages)
                {
                    EndGame(EndGameTyp.AllSurvivorInCage);
                    break;
                }
            }
           
            yield return null;
        }
    }
    
    #region Game End
    public void GameEndsOnline(EndGameTyp endGameTyp)
    {
        if (IsOwner)
        {
            EndGame(endGameTyp);
            EndGameServerRpc(endGameTyp);
        }
    }
    
    [ServerRpc]
    private void EndGameServerRpc(EndGameTyp endGameTyp)
    {
        EndGameClientRpc(endGameTyp);
    }
    [ClientRpc]
    private void EndGameClientRpc(EndGameTyp endGameTyp)
    {
        if(!IsOwner) EndGame(endGameTyp);
    }
    private void EndGame(EndGameTyp endGameTyp)
    {
        var win = false;
        switch (endGameTyp)
        {
            case EndGameTyp.TimeOut:
                win = myMonster != null;
                break;
            case EndGameTyp.AllTheSurvivorsRuns:
                win = myMonster == null;
                break;
            case EndGameTyp.AllSurvivorInCage:
                win = myMonster != null;
                break;
        }
        State = win ? GameState.Win : GameState.Lose;
        UIManager.Instance.panels.SetPanel(State);
    }

    
    #endregion
}
