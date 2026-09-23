using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool
{
    private readonly GameObject prefab;
    private readonly Queue<GameObject> available = new Queue<GameObject>();
    private readonly List<GameObject> active = new List<GameObject>();

    public BulletPool(GameObject prefab,int initialSize)
    {
        this.prefab = prefab;
        for (int i = 0; i < initialSize; i++) CreateNew();
    }

    private void CreateNew()
    {
        GameObject obj = Object.Instantiate(prefab);
        obj.SetActive(false);
        available.Enqueue(obj);

    }

    public GameObject Get()
    {
        GameObject obj = available.Count > 0 ? available.Dequeue() : Object.Instantiate(prefab);
        obj.SetActive(true);
        active.Add(obj);
        return obj;

    }

    public void Release(GameObject obj)
    {
        obj.SetActive(false);
        available.Enqueue(obj);
        active.Remove(obj);
    }

    public void ClearActive()
    {
        foreach(GameObject obj in active)
        {
            obj.SetActive(false);
            available.Enqueue(obj);
        }
        active.Clear();
    }
}
