using System;
using UnityEngine;

namespace Game.Generator
{
    public class Generator : MonoBehaviour
    {
        [SerializeField] private new Renderer renderer;
        private bool active;
        private int myLayer;
        
        private void Start()
        {
            myLayer = gameObject.layer;
        }
        

        public void InUse()
        {
            gameObject.layer = 0;
        }

        public void CancelUse()
        {
            gameObject.layer = myLayer;
        }

        public void Repaired()
        {
            active = true;
            renderer.material.SetColor("_RimColor", Color.black);
        }
        public bool IsActive() => active;
    }
}
