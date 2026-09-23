using System;
using System.Collections.Generic;


public class ObjectPool<T>
{
    private readonly Queue<T> available = new Queue<T>();
    private readonly List<T> active = new List<T>();

    private readonly Func<T> createFunc;
    private readonly Action<T> actionOnGet;
    private readonly Action<T> actionOnRelease;
    private readonly Action<T> actionOnDestroy;

    public ObjectPool(Func<T> createFunc, Action<T> actionOnGit, Action<T> actionOnRelease, Action<T> actionOnDestroy, int initialSize)
    {
        this.createFunc = createFunc;
        this.actionOnGet = actionOnGit;
        this.actionOnRelease = actionOnRelease;
        this.actionOnDestroy = actionOnDestroy;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = createFunc();
            actionOnRelease?.Invoke(obj);
            available.Enqueue(obj);
        }
    }

    

    public T Get()
    {
        T obj = available.Count > 0 ? available.Dequeue() : createFunc();

        actionOnGet?.Invoke(obj);
        active.Add(obj);
        return obj;
    }

    public void Release(T obj)
    {
        actionOnRelease?.Invoke(obj);
        
        available.Enqueue(obj);
        active.Remove(obj);
    }

    public void ClearActive()
    {
        foreach(T obj in active)
        {
            actionOnRelease?.Invoke(obj);
            available.Enqueue(obj);
        }
        active.Clear();
    }
}
