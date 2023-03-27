using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ItemType
    {
        Sword, 
        Helmet,
        Shoulder,
        Pants,
        Shoes,
        Chest,
    }

    public ItemType itemType;
    public int amount;
    
    
}
