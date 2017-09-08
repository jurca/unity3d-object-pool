using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool {
    private static uint lastInstanceId = 0;

    public int minCapacity;
    public int maxCapacity;
    public GameObject prefab;
    public bool returnNullOnUnderflow;
    public Func<GameObject, GameObject> initializer = (prefab) => GameObject.Instantiate (prefab);
    public string label = "<Unlabeled object pool #" + ++ObjectPool.lastInstanceId + ">";

    private GameObject[] pool;
    private bool ready = false;
    private int queueStart = 0;
    private int queueEnd = 0;
    private int objectsInPool = 0;

    public static bool IsPooled(GameObject gameObject) {
        return gameObject.GetComponent<PooledObjectController> ();
    }

    public static void ReturnObjectToPool(GameObject usedObject) {
        usedObject.GetComponent<PooledObjectController> ().ReturnToPool ();
    }

    public GameObject GetObject() {
        if (!ready) {
            Init ();
        }

        if (objectsInPool == 0) {
            if (pool.LongLength < maxCapacity) {
                Grow ();
            } else if (returnNullOnUnderflow) {
                return null;
            } else {
                throw new OutOfMemoryException(
                    "The maximum capacity (" + maxCapacity + ") of the object pool '" + label + "' has been exceeded"
                );
            }
        }

        GameObject objectInstance = pool[queueStart];
        objectInstance.SetActive (true);
        queueStart = (queueStart + 1) % pool.Length;
        objectsInPool--;
        return objectInstance;
    }

    public void ReturnObject(GameObject usedObject) {
        if (!ready) {
            Init ();
        }

        usedObject.SetActive (false);
        pool [queueEnd] = usedObject;
        queueEnd = (queueEnd + 1) % pool.Length;
        objectsInPool++;
    }

    public void Init() {
        if (ready) {
            return;
        }
        ready = true;

        if (prefab == null) {
            throw new ArgumentNullException ("The prefab game object cannot be null");
        }
        if (initializer == null) {
            throw new ArgumentNullException ("The game object instance initializer function cannot be null");
        }
        if (minCapacity <= 0) {
            throw new ArgumentOutOfRangeException ("The minimum capacity must be a positive integer");
        }
        if (maxCapacity < minCapacity) {
            throw new ArgumentOutOfRangeException (
                "The maximum capacity (" + maxCapacity + ") cannot be lower than the minimum capacity (" + minCapacity + ")"
            );
        }

        pool = new GameObject[minCapacity];
        for (int i = 0; i < minCapacity; i++) {
            AddObjectInstance ();
        }
    }

    private void Grow() {
        Debug.LogWarning (
            "The currenct capacity (" + pool.LongLength + ") of the pool '" + label +
            "' has been exceeded, increasing capacity"
        );

        int newCapacity;
        if (pool.Length > maxCapacity / 2) {
            newCapacity = maxCapacity;
        } else {
            newCapacity = pool.Length * 2;
        }
        int capacityIncrease = newCapacity - pool.Length;
        pool = new GameObject[newCapacity];

        for (int i = 0; i < capacityIncrease; i++) {
            AddObjectInstance ();
        }
    }

    private void AddObjectInstance() {
        GameObject objectInstance = initializer (prefab);
        PooledObjectController controller = objectInstance.GetComponent<PooledObjectController> ();
        controller.parentPool = this;
        ReturnObject (objectInstance);
    }
}
