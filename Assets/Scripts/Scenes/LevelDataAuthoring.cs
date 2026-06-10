using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Components;

public class LevelDataAuthoring : MonoBehaviour
{
    [Tooltip("Must match the exact Scene name, e.g., 'Level_Hell'")]
    public string levelName; 

    class Baker : Baker<LevelDataAuthoring>
    {
        public override void Bake(LevelDataAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(entity, new LevelSceneTag 
            { 
                LevelName = new FixedString64Bytes(authoring.levelName) 
            });
            AddComponent(entity, new ActiveSceneEntity());
        }
    }
}