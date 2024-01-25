using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Generator
{
    public class GeneratorUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI generatorText;
        [SerializeField] private GameObject generatorIcon;
        private List<Generator> generators = new List<Generator>();

        
        private void Start()
        {
            foreach (var g in FindObjectsOfType<Generator>())
            {
                generators.Add(g);
            }
        }

        private void Update()
        {
            generatorIcon.SetActive(GeneratorRequired()>0);
            generatorText.text = GeneratorRequired()>0? GeneratorRequired() + " generator not repaired":
                    "The door is open run";
        }

        private int GeneratorRequired()
        {
            var count = 0;
            for (int i = 0; i < generators.Count; i++)
            {
                if (!generators[i].IsActive()) count++;
            }
            return count;
        }
    }
}
