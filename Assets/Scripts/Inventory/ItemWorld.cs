using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    [SerializeField] private Item item;

    private void Awake()
    {
        item = new Item(item.itemType);
    }

    public static ItemWorld SpawnItemWorld(Vector3 _spawnPoint, Item _item)
    {
        Transform transform = Instantiate(_item.worldPrefab, _spawnPoint, Quaternion.identity);

        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.SetItem(_item);

        return itemWorld;
    }
    
    public void SetItem(Item _item)
    {
        item = _item;
    }

    public Item GetItem()
    {
        return item;
    }
}
