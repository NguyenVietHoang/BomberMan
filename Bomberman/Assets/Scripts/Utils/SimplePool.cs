using System.Collections.Generic;
using UnityEngine;

public class SimplePool: MonoBehaviour
{
    public GameObject m_prefab;

    // Size of this object pool
    public int m_size;

    // The list of free and used objects for tracking.
    // We use the generic collections so we can give them our type T.
    private Queue<GameObject> poolList;

    public void InitPool(int size, GameObject prefab)
    {
        poolList = new Queue<GameObject>();
        m_size = size;
        m_prefab = prefab;

        // Instantiate the pooled objects and disable them.
        for (var i = 0; i < m_size; i++)
        {
            GameObject pooledObject = Instantiate(m_prefab, transform);
            pooledObject.SetActive(false);
            poolList.Enqueue(pooledObject);
        }
    }

    public GameObject Spawn(Transform parent, Vector3 newPos, Quaternion newRot)
    {
        var numFree = poolList.Count;
        if (numFree == 0)
            return null;

        // Pull an object from the end of the free list.
        var pooledObject = poolList.Dequeue();
        
        pooledObject.transform.position = newPos;
        pooledObject.transform.rotation = newRot;
        pooledObject.transform.SetParent(parent, true);
        pooledObject.SetActive(true);
        return pooledObject;
    }

    public void DeSpawn(GameObject pooledObject)
    {
        // Put the pooled object back in the free list.
        poolList.Enqueue(pooledObject);

        // Reparent the pooled object to us, and disable it.
        var pooledObjectTransform = pooledObject.transform;
        pooledObjectTransform.parent = transform;
        pooledObjectTransform.localPosition = Vector3.zero;
        pooledObject.SetActive(false);
    }
}