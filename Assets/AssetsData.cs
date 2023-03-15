using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AssetInfo
{
    public string id;
    public GameObject prefab;
}

[CreateAssetMenu(menuName = "Custom/AssetList")]
public class AssetsData : ScriptableObject
{
    [SerializeField] private List<AssetInfo> AssetsList;

    public bool GetAsset(string _id, out AssetInfo value)
    {
        foreach (var asset in AssetsList)
        {
            if (asset.id == _id)
            {
                value = asset;
                return true;
            }
        }
        value = default;
        return false;
    }

    public List<AssetInfo> GetAllAssets()
    {
        if (AssetsList != null)
        {
            return AssetsList;
        }
        
        return null;
    }
}
