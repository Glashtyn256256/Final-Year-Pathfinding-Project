using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MinHeap<T> where T : IHeapItem<T>
{
    //private List<T> data = new List<T>();
    T[] data;
    int currentItemAmount;

    /*These return us the parent of the item, the left child index if there is one and the right child index
     if there is one*/
    private int GetParentIndex(int itemindex) { return (itemindex - 1) / 2; }
    private int GetLeftChildIndex(int itemindex) { return 2 * itemindex + 1; }
    private int GetRightChildIndex(int itemindex) { return 2 * itemindex + 2; }

    private bool HasLeftChild(int itemindex) { return GetLeftChildIndex(itemindex) < currentItemAmount; }
    private bool HasRightChild(int itemindex){ return GetRightChildIndex(itemindex) < currentItemAmount; }
    private bool IsRoot(int itemindex) { return itemindex == 0; }

    private T GetParent(int itemindex) { return data[GetParentIndex(itemindex)]; }
    private T GetLeftChild(int itemindex) { return data[GetLeftChildIndex(itemindex)]; }
    private T GetRightChild(int itemindex) { return data[GetRightChildIndex(itemindex)]; }


    //public T Last()
    //{
    //    return 
    //}

    public MinHeap(int arraysize)
    {
        data = new T[arraysize];
    }
    public int Count()
    {
        return currentItemAmount;
    }

    public T PeekFrontItem()
    {
        return data[0];
    }

    public bool Contains(T item)
    {
        return Equals(data[item.HeapIndex], item);
    }

    public List<T> ToList()
    {
        List<T> TList = new List<T>();
        TList.AddRange(data);
        return TList;
    }

    /*This will return the Object at the end of the heap and 
     remove it from the heap.*/
    public T RemoveFrontItem()
    {
        if (currentItemAmount == 0)
        {
            throw new IndexOutOfRangeException();
        }

        T frontItem = data[0];
        data[0] = data[currentItemAmount - 1];
        data[0].HeapIndex = 0;
        currentItemAmount--;
        ReCalculateDown(data[0]);

        return frontItem;
    }

    public void Add(T item)
    {
        //Throw exception if there is a error.
        if (currentItemAmount == data.Length)
        {
            throw new IndexOutOfRangeException();
        }

        /* set the items current position in the array*/
        item.HeapIndex = currentItemAmount;
        /*store the item in the array using the current item count*/
        data[currentItemAmount] = item;
        /*increment the item count*/
        currentItemAmount++;

        /*Recalculate the heap to see which position the new item 
        should be in in the heap.*/
        ReCalculateUp(item);
    }

    //public void UpdateItem(T item)
    //{
    //    ReCalculateUp(item);
    //}

    void ReCalculateDown(T item)
    {
        while (HasLeftChild(item.HeapIndex))
        {
            int changeIndex = GetLeftChildIndex(item.HeapIndex);

            if (HasRightChild(item.HeapIndex)
            && GetLeftChild(item.HeapIndex).CompareTo(GetRightChild(item.HeapIndex)) < 0)
            {
                changeIndex = GetRightChildIndex(item.HeapIndex);
            }
            if (item.CompareTo(data[changeIndex]) >= 0)
            {
                return;
            }

            Swap(item, data[changeIndex]);
        }
    }
    private void ReCalculateUp(T item)
    {
        /*We want to swap when the item isn't the top of the binary tree and
        and the new item added is less than it's parent.*/
        while (!IsRoot(item.HeapIndex) && data[item.HeapIndex].CompareTo(GetParent(item.HeapIndex)) > 0)
        {
            //Swaps the positions of the items;
            Swap(data[item.HeapIndex], GetParent(item.HeapIndex));
        }
    }
   
    private void Swap(T itemA,T itemB)
    {
        data[itemA.HeapIndex] = itemB;
        data[itemB.HeapIndex] = itemA;

        int tempItemIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = tempItemIndex;
        
    }
}

/*Make sure that the item we are passing through has to have a heapindex.
This was used as a reference from sebsation leagues min heap tutorial */
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
