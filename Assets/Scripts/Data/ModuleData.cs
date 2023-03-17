using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/ModulesList")]
public class ModuleData : ScriptableObject
{
    [SerializeField] private List<ModuleObject> ModuleList;
    
    public bool GetAsset(string _id, out ModuleObject value)
    {
        foreach (var asset in ModuleList)
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

    public bool GetAllAssets(out List<ModuleObject> value)
    {
        if (ModuleList != null)
        {
            value = ModuleList;
            return true;
        }

        value = default;
        return false;
    }

    public bool GetDictionary(out Dictionary<string, ModuleObject> value)
    {
        if (ModuleList != null)
        {
            Dictionary<string, ModuleObject> modulesDictionary = new Dictionary<string, ModuleObject>();
            foreach (var module in ModuleList)
            {
                modulesDictionary.Add(module.id, module);
            }

            if (modulesDictionary.Count == ModuleList.Count)
            {
                value = modulesDictionary;
                return true;
            }
            else
            {
                Debug.Log("Dictionary entry amount don't match");
                value = default;
                return false;
            }
            
        }
        
        Debug.Log("Couldn't get dictionary for Modules");
        value = default;
        return false;
    }
}
