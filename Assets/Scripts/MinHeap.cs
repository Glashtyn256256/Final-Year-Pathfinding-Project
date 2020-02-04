using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MinHeap<T> where T : IComparable<T>
{
    private List<T> data = new List<T>();
    
    //public T Last()
    //{
    //    return 
    //}
    public int Count()
    {
        return data.Count;
    }
    public T FrontItem()
    {
        return data[0];
    }
    public void Add(T item)
    {
        data.Add(item);
    }

    public bool Contains(T item)
    {
        return data.Contains(item);
    }

    public void Remove(T item)
    {
        data.Remove(item);
    }

    public List<T> ToList()
    {
        return data;
    }
    //Get Last Value.
    //Get Front value
    //GetSizeCount.
}
