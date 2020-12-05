using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] private int _rows;
    public int rows { get => _rows; }
    [SerializeField] private int _columns;
    public int columns { get => _columns; }

    [SerializeField] protected float massCurrent;
    [SerializeField] protected float volumeCurrent;
    [SerializeField] protected float massMAX;
    [SerializeField] protected float volumeMAX;

    //private Storable[,] _storedItems;
    //public Storable[,] storedItems { get => _storedItems; }

    [SerializeField] private Storable[] _storedItems;
    public Storable[] storedItems { get => _storedItems; }

    //private int count = 0;

    // For demo/debug purposes.
    //[SerializeField] private List<Storable> startingItems;

    void OnValidate()
    {
        if (_rows > 0 && _columns > 0)
        {
            if (_storedItems == null)
                _storedItems = new Storable[_rows * _columns];
            // if (_storedItems == null || _storedItems.GetLength(0) != _rows || _storedItems.GetLength(1) != _columns)
            //_storedItems = new Storable[_rows, _columns];

            //Debug.Assert(startingItems.Count <= _rows * _columns);
            //for (int i = 0; i < startingItems.Count; i++)
            //{
            //    int x = i / _columns;
            //    int y = i % _columns;
            //    AddItem(startingItems[i], x, y);
            //}
        }
        else
        {
            Debug.Log("Storage: OnValidate(): rows and/or columns are not set to" +
                      " valid integers.");
        }
    }

    public void Start()
    {
        //_storedItems = new Storable[_rows, _columns];
    }

    public bool AddItem(Storable s)
    {
        //int x, y;
        //if (GetEmptySlot(out x, out y))
        int index;
        if (GetEmptySlot(out index))
        {
            _storedItems[index] = s;
            s.DeactivateInWorld();
            //count++;
            massCurrent += s.objectPhysicalStats.mass;
            volumeCurrent += s.objectPhysicalStats.volume;
            return true;
        }
        return false;
    }

    //public bool AddItem(Storable s, int x, int y)
    //{
    //    if (count < _rows * _columns && x <= _rows && y <= _columns)
    //    {
    //        if (_storedItems[x, y] == null)
    //        {
    //            if (_storedItems[x, y] == s)
    //                return true;                                                // TODO: It should return here, but is it correct to return true here? 
    //            _storedItems[x, y] = s;
    //            s.DeactivateInWorld();
    //            count++;
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    public bool AddItem(Storable s, int index)
    {
        if (index < _storedItems.Length)
        {
            if (_storedItems[index] == null)
            {
                _storedItems[index] = s;
                if (s != null)
                {
                    s.DeactivateInWorld();
                    massCurrent += s.objectPhysicalStats.mass;
                    volumeCurrent += s.objectPhysicalStats.volume;
                }
                //count++;
                return true;
            }
        }
        return false;
    }

    //public Storable RemoveItem(int x, int y)
    //{
    //    if (x <= _rows && y <= _columns)
    //    {
    //        Storable s = _storedItems[x, y];
    //        _storedItems[x, y] = null;
    //        count--;
    //        return s;
    //    }
    //    return null;
    //}

    public Storable RemoveItem(int index)
    {
        if (index <= _storedItems.Length)
        {
            Storable s = _storedItems[index];
            _storedItems[index] = null;
            if (s != null)
            {
                massCurrent -= s.objectPhysicalStats.mass;
                volumeCurrent -= s.objectPhysicalStats.volume;
            }
            return s;
        }
        return null;
    }

    public bool GetEmptySlot(out int index)
    {
        for (int i = 0; i < _storedItems.Length; i++)
        {
            if (_storedItems[i] == null)
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }


    //public bool GetEmptySlot(out int x, out int y)
    //{
    //    if (count < _rows * _columns)
    //    {
    //        for (int i = 0; i < _rows; i++)
    //        {
    //            for (int j = 0; j < _columns; j++)
    //            {
    //                if (_storedItems[i, j] == null)
    //                {
    //                    x = i;
    //                    y = j;
    //                    return true;
    //                }
    //            }
    //        }
    //    }
    //    x = -1;
    //    y = -1;
    //    return false;
    //}

    public bool StorableFitsInStorage(Storable storable)
    {
        if (storable == null)
            return true;                                                        // Is this correct? Do we want false instead?
        float m = storable.objectPhysicalStats.mass;
        float v = storable.objectPhysicalStats.volume;
        if (massCurrent + m <= massMAX && volumeCurrent + v < volumeMAX)
            return true;
        else
            return false;
    }

}
