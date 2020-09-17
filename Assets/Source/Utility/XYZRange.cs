using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct XYZRange
{
    // NOTE: XYZRange is INCLUSIVE! 
    // In other words, any point (x,y,z) where: 
    //     Range-Mins <= (x,y,z) <= Range-Maxes 
    // would be considered to be within the range.

    [SerializeField] private float _xMin;
    public float xMin
    {
        get => _xMin;
        set { _xMin = (value <= _xMax) ? value : _xMin; }
    }

    [SerializeField] private float _xMax;
    public float xMax
    {
        get => xMax;
        set { xMax = (value >= _xMin) ? value : _xMax; }
    }

    [SerializeField] private float _yMin;
    public float yMin
    {
        get => _yMin;
        set { _yMin = (value <= _yMax) ? value : _yMin; }
    }

    [SerializeField] private float _yMax;
    public float yMax
    {
        get => yMax;
        set { yMax = (value >= _yMin) ? value : _yMax; }
    }

    [SerializeField] private float _zMin;
    public float zMin
    {
        get => _zMin;
        set { _zMin = (value <= _zMax) ? value : _zMin; }
    }

    [SerializeField] private float _zMax;
    public float zMax
    {
        get => zMax;
        set { zMax = (value >= _zMin) ? value : _zMax; }
    }

    // Constructor w/data validation
    public XYZRange(float xmin, float xmax,
                    float ymin, float ymax,
                    float zmin, float zmax )
    {
        if (xmin < xmax)
        {
            if (ymin < ymax)
            {
                if (zmin < zmax)
                {
                    _xMin = xmin;
                    _xMax = xmax;
                    _yMin = ymin;
                    _yMax = ymax;
                    _zMin = zmin;
                    _zMax = zmax;
                }
                else
                {
                    throw new UnityEngine.UnityException("XYZRange: zMin cannot be larger than zMax!");
                }
            }
            else
            {
                throw new UnityEngine.UnityException("XYZRange: yMin cannot be larger than yMax!");
            }
        }
        else
        {
            throw new UnityEngine.UnityException("XYZRange: xMin cannot be larger than xMax!");
        }
    }

    public bool ContainsXValue(float xVal)
    {
        return (xVal >= _xMin && xVal <= _xMax);
    }

    public bool ContainsYValue(float yVal)
    {
        return (yVal >= _yMin && yVal <= _yMax);
    }

    public bool ContainsZValue(float zVal)
    {
        return (zVal >= _zMin && zVal <= _zMax);
    }

    public bool ContainsPoint(Vector3 point)
    {
        return (   point.x >= _xMin
                && point.x <= _xMax
                && point.y >= _yMin
                && point.y <= _yMax
                && point.z >= _zMin
                && point.z <= _zMax);
    }

    public bool ContainsRange(XYZRange other)
    {
        return (   other.xMin >= _xMin
                && other.xMax <= _xMax
                && other.yMin >= _yMin
                && other.yMax <= _yMax
                && other.zMin >= _zMin
                && other.zMax <= _zMax);
    }
}
