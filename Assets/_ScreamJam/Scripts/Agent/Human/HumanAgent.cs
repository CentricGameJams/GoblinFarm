using System;
using UnityEngine;

namespace _ScreamJam.Scripts.Agent
{
    public class HumanAgent : AgentBase
    {
        protected override void Start()
        {
            IsHuman = true;
            base.Start();
        }
        
    }
}