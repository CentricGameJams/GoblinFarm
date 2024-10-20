using System;
using _ScreamJam.Scripts.Scriptables;
using UnityEngine;

namespace _ScreamJam.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public GameObjectListSO Humans;
        public GameObjectListSO Goblins;

        private void Awake()
        {
            Humans.Value.Clear();
            Goblins.Value.Clear();
        }
        
        //load & unload new scenes
        void OnEnterGate()
        {
            
        }
    }
}