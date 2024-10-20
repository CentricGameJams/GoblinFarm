using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace _ScreamJam.Scripts
{
    public static class GameplayUtils
    {
        public static GameObject GetClosest(Transform transform, List<GameObject> gos, float minDist = float.MaxValue)
        {
            float minDistSQ = minDist * minDist;
            Vector3 pos = transform.position;
            GameObject closest = null;
            foreach (var go in gos)
            {
                float d = math.distancesq(go.transform.position, pos);
                if (d < minDistSQ)
                {
                    closest = go;
                    minDistSQ = d;
                }
            }
            return closest;
        }
    }
}