using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ItemsAsset")]
public class ItemAssets : ScriptableObject
{
    [SerializeField] private List<SpriteData> spriteAssets;

    public Sprite GetSprite(Item.ItemType _spriteType)
    {
        Sprite sprite = null;
        
        foreach (var data in spriteAssets)
        {
            if (data.itemType == _spriteType)
            {
                sprite = data.sprite;
            }
        }

        return sprite;
    }

    public Transform GetWorldPrefab(Item.ItemType _prefabType)
    {
        Transform prefab = null;
        foreach (var data in spriteAssets)
        {
            if (data.itemType == _prefabType)
            {
                prefab = data.worldPrefab;
            }
        }

        return prefab;
    }
}

[System.Serializable]
public class SpriteData
{
    public Item.ItemType itemType;
    public Sprite sprite;
    public Transform worldPrefab;
}
