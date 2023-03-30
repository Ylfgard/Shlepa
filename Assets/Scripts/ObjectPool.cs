using UnityEngine;
using System.Collections.Generic;

public class ObjectPool<T> where T : MonoBehaviour
{
    private GameObject _prefab;
    private List<T> _objects;

    public T Value => _prefab.GetComponent<T>();

    public ObjectPool(GameObject prefab)
    {
        _prefab = prefab;
        _objects = new List<T>();
    }

    public T GetObjectFromPool()
    {
        foreach (T obj in _objects)
            if (obj.gameObject.activeSelf == false)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }

        var newObj = Object.Instantiate(_prefab).GetComponent<T>();
        _objects.Add(newObj);
        newObj.gameObject.SetActive(true);
        return newObj;
    }
}
