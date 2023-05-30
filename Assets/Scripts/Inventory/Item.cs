using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public enum ItemType
    {
        Sword, 
        Helmet,
        Pants,
        Chest,
        RedPotion,
        YellowPotion,
    }
    
    public ItemType itemType;
    public Sprite itemSprite;
    public Transform worldPrefab;

    private ItemAssets itemAssets;
    
    public Item(ItemType _itemType)
    {
        itemType = _itemType;
        itemAssets = Resources.Load("ItemsAsset") as ItemAssets;
        itemSprite = itemAssets.GetSprite(itemType);
        worldPrefab = itemAssets.GetWorldPrefab(itemType);
    }
}
