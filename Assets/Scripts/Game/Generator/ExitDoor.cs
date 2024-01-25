using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Generator
{
    public class ExitDoor : MonoBehaviour
    {
        [SerializeField] private Transform door;
        [SerializeField] private Transform openDorePos;
        [SerializeField] private new GameObject camera;
        private readonly List<Generator> generators = new List<Generator>();

        private void Start()
        {
            foreach (var g in FindObjectsOfType<Generator>())
            {
                generators.Add(g);
            }
            camera.SetActive(false);
        }

        private void Update()
        {
            if(AllTheGeneratorActive()) Open();
        }

        private bool AllTheGeneratorActive()
        {
            var no = true;
            for (int i = 0; i < generators.Count; i++)
            {
                if (!generators[i].IsActive()) no = false;
            }
            return no;
        }

        private void Open()
        {
            camera.SetActive(true);
            Destroy(camera,2f);
            enabled = false;
            door.DOMove(openDorePos.position, 0.7f);
            door.DORotate(openDorePos.eulerAngles, 0.7f);
        }
    }
}
