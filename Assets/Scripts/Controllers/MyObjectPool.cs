using System;
using System.Collections.Generic;
using Prototype.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MyObjectPool : MonoBehaviour, IControllable
{
    List<Set> pool = new List<Set>();

    [Header("Refrences")] [SerializeField] private SetSpawner _setSpawner;

    [Header("Settings")]
    [SerializeField] private int poolSizePerSet = 10;

    [Header("Overwrite sets - place all sets here")]
    [SerializeField] private Set[] _overWriteSets;


    private GameObject _poolContainer;
    
    public void Initialize()
    {
        LoadPool();
        _poolContainer = new GameObject("PoolContainer");
        _poolContainer.transform.SetParent(transform);
    }

    public void DoUpdate()
    {
        throw new NotImplementedException();
    }
    
    public void DoFixedUpdate(){}

    public Set GetRandomSet()
    {
        if (pool.Count == 0)
            return null;
        Set foundSet = null;
        while (foundSet == null)
        {
            int myRandomFuckingNumber = Random.Range(0, pool.Count);
            Set pulledSet = pool[myRandomFuckingNumber];
            
            float chance = pulledSet.SpawnChance;
            if (Random.value >= chance) // adds chance for a set to spawn
                continue;
            if (pulledSet == _setSpawner.LatestSpawnedSet && Random.value > 0.8f)  // decrease likelyhood of repeat sets
            {
                Debug.LogError("Latest set is identical");
                continue;
            }
            
            foundSet = pulledSet;
        }

        pool.Remove(foundSet);
        foundSet.gameObject.SetActive(true);
        return foundSet;
    }

    public void AddBackToPool(Set set)
    {
        set.gameObject.SetActive(false);
        pool.Add(set);
    }

    private void LoadPool()
    {
        // load overwrited sets if else
        Set[] loadedSets;
        if(_overWriteSets != null && _overWriteSets.Length > 0)
            loadedSets = _overWriteSets;
        else
            loadedSets = Resources.LoadAll<Set>("Sets");
        
        foreach (Set set in loadedSets)
        {
            for (int i = 0; i < poolSizePerSet; i++)
            {
                Set newSet = CreateSet(set);
                pool.Add(newSet);
            } 
        }
    }

    private Set CreateSet(Set _set)
    {
       Set set = Instantiate(_set); 
       set.gameObject.SetActive(false);
       return set;
    }

}
