using System;
using Unity.Mathematics;
using UnityEngine;

namespace _ScreamJam.Scripts.Agent
{
    public interface IDamageable
    {
        public void TakeDamage(int dmg);
    }

    public interface IInteractable
    {
        public void Interact();
    }

    public interface IPickUp
    {
        public void Pickup();
    }

    public class Weapon : MonoBehaviour, IPickUp
    {
        public int Damage;
        public Rigidbody Rb;
        public float ThrowForce;
        public float AngularSpeed;

        public void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent<IDamageable>(out var dmgable))
            {
                dmgable.TakeDamage(Damage);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<IDamageable>(out var dmgable))
            {
                dmgable.TakeDamage(Damage);
            }
        }

        public void TakeDamage(AgentBase agent)
        {
            agent.Health -= Damage;
        }

        public void Pickup()
        {
            Rb.useGravity = false;
            Rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        
        public void Throw(Vector3 dir, Vector3 spinDir)
        {
            transform.parent = null;
            Rb.constraints = RigidbodyConstraints.None;
            Rb.AddForce(dir * ThrowForce, ForceMode.Impulse);
            Rb.angularVelocity = math.PI2 * AngularSpeed * spinDir;
            Rb.useGravity = true;
            Rb.freezeRotation = false;
        }
        
    }
}