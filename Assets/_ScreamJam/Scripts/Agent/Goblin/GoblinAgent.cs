using UnityEngine;

namespace _ScreamJam.Scripts.Agent
{
    public class GoblinAgent : AgentBase
    {
        protected override void Start()
        {
            IsHuman = false;
            base.Start();
        }
        
        public void Free()
        {
            _State = AgentState.Idle;
            transform.parent = null;
        }

        public void Imprison()
        {
            _State = AgentState.Imprisoned;
        }
    }
}