using System;
using System.Collections;
using System.Collections.Generic;
using _ScreamJam.Scripts.Agent;
using UnityEngine;

namespace _ScreamJam.Scripts.Gameplay
{
    public class Prison : MonoBehaviour, IDamageable
    {
        public int Health;
        public GameObject WallRoot;
        public List<GoblinAgent> Prisoners;
        public ParticleSystem BreakPS;

        private void Start()
        {
            foreach (var p in Prisoners)
            {
                p.Imprison();
            }
        }

        private void Update()
        {
            
        }

        IEnumerator BreakCo()
        {
            if(BreakPS is not null)
                BreakPS.Play();
            foreach (var p in Prisoners)
            {
                p.Free();
            }
            WallRoot.SetActive(false);
            yield return new WaitForSeconds(2.0f);
            
            Destroy(gameObject);
            yield return null;
        }
        
        void Break()
        {
            StartCoroutine(BreakCo());
        }
        
        public void TakeDamage(int dmg)
        {
            Health -= dmg;
            if (Health <= 0)
            {
                Break();
            }
            
        }
    }
}