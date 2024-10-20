using UnityEngine;

namespace _ScreamJam.Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "GORef", menuName = "ScreamJam/GameObjRefSO", order = 0)]
    public class GameObjectReferenceSO : ScriptableObject
    {
        public GameObject GO;
        public Transform Transform;
    }
}