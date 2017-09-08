using System;
using UnityEngine;

public static class PooledObjectExtensions {
    public static bool IsPooled(this GameObject gameObject) {
        return ObjectPool.IsPooled (gameObject);
    }

    public static void ReturnToPool(this GameObject gameObject) {
        ObjectPool.ReturnObjectToPool (gameObject);
    }
}
