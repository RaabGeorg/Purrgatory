using Unity.Mathematics;
using UnityEngine;

namespace Combat.Magic
{
    public class ExplosionVFXSpawner : MonoBehaviour
    {
        public static ExplosionVFXSpawner instance;

        public GameObject RedExplosionPrefab;
        public GameObject PurpleExplosionPrefab;
        public GameObject LightExplosionPrefab;

        void Awake()
        {
            instance = this;
        }
        
        public void Spawn(float3 pos,int decision)
        {
            switch (decision)
            {
                case 0:
                    Instantiate(PurpleExplosionPrefab, pos, Quaternion.identity);
                    break;
                case 1:
                    Instantiate(RedExplosionPrefab, pos, Quaternion.identity);
                    break;
                case 2:
                    Instantiate(LightExplosionPrefab, pos, Quaternion.identity);
                    break;
                default:
                    Instantiate(PurpleExplosionPrefab, pos, Quaternion.identity);
                    break;
            }
        }
    }
}