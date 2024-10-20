using System.Collections.Generic;
using UnityEngine;

namespace _ScreamJam.Scripts.Scriptables
{
    
    [CreateAssetMenu(fileName = "GameObjListSO", menuName = "ScreamJam/", order = 0)]
    public class GameObjectListSO : ScriptableObject
    {
        public List<GameObject> Value;

        public void Add(GameObject obj)
        {
            Value.Add(obj);
        }
        public void Remove(GameObject obj)
        {
            Value.Remove(obj);
        }
    }
}