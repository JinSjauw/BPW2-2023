using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private Item.ItemType itemType;
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnItem();
    }

    public void SpawnItem()
    {
        Item item = new Item(itemType);
        ItemWorld.SpawnItemWorld(transform.position, item);
        Debug.Log("Spawning: " + item.worldPrefab.name);
        Destroy(gameObject);
    }
}
