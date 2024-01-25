using System;
using UnityEngine;

namespace Start
{
    public enum GameState
    {
        Start, Options, Loading, Cosmetics
    }
    [Serializable]
    public class Objects
    {
        [SerializeField] private GameObject[] startObjects, optionsObjects, loadingObject, cosmeticsObjects;

        public void SetObjects(GameState state)
        {
            for (int i = 0; i < startObjects.Length; i++)
            {
                startObjects[i].SetActive(false);
            }
            for (int i = 0; i < optionsObjects.Length; i++)
            {
                optionsObjects[i].SetActive(false);
            }
            for (int i = 0; i < loadingObject.Length; i++)
            {
                loadingObject[i].SetActive(false);
            }
            for (int i = 0; i < cosmeticsObjects.Length; i++)
            {
                cosmeticsObjects[i].SetActive(false);
            }

            switch (state)
            {
                case GameState.Start: 
                    for (int i = 0; i < startObjects.Length; i++)
                    {
                        startObjects[i].SetActive(true);
                    }
                    break;
                case GameState.Options:
                    for (int i = 0; i < optionsObjects.Length; i++)
                    {
                        optionsObjects[i].SetActive(true);
                    }
                    break;
                case GameState.Loading:
                    for (int i = 0; i < loadingObject.Length; i++)
                    {
                        loadingObject[i].SetActive(true);
                    }
                    break;
                case GameState.Cosmetics:
                    for (int i = 0; i < cosmeticsObjects.Length; i++)
                    {
                        cosmeticsObjects[i].SetActive(true);
                    }
                    break;
            }
        }
    }
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance {get; private set;}
        public static GameState State;
        [SerializeField] private Objects startObject;
        

        private void Awake()
        {
            Application.targetFrameRate = 120;
            Instance = this;
            SetGameState(GameState.Start);
        }

        public void SetGameState(GameState state)
        {
            State = state;
            startObject.SetObjects(State);
        }
    }
}
