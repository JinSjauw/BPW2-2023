using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    private enum LootType
    {
        POTIONS,
        EQUIPMENT,
    }
    
    private Item.ItemType itemType;
    [SerializeField] private Transform lid;
    [SerializeField] private LootType lootType;
    private bool isOpen = false;
    private bool inRange = false;
    private void Awake()
    {
        if (lootType == LootType.EQUIPMENT)
        {
            itemType = (Item.ItemType)Random.Range(0, (int)Item.ItemType.Chest);
        }
        else
        {
            itemType = (Item.ItemType)Random.Range((int)Item.ItemType.RedPotion, (int)Item.ItemType.YellowPotion);
        }
    }

    private void OnMouseOver()
    {
        
    }

    public void OnClicked()
    {
        if (!isOpen)
        {
            StartCoroutine(OpenChest());
        }
    }
    
    private void SpawnItem()
    {
        Item item = new Item(itemType);

        Vector3 dropPosition = transform.position;
        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = Random.Range(1, 5);
        randomDirection.Normalize();
        ItemWorld itemWorld = ItemWorld.SpawnItemWorld(dropPosition + randomDirection * 2f, item);
        itemWorld.GetComponentInParent<Rigidbody>().AddForce(randomDirection * 3f, ForceMode.Impulse);
    }

    private IEnumerator OpenChest()
    {
        isOpen = true;
        for (int rotX = 0; rotX < 10; rotX++)
        {
            lid.transform.Rotate(new Vector3(rotX * -1, 0, 0));
            yield return null;
        }
        
        SpawnItem();
    }
}
