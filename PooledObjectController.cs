using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObjectController : MonoBehaviour {
    [HideInInspector] public ObjectPool parentPool;

    public void ReturnToPool() {
        parentPool.ReturnObject (this.gameObject);
    }
}
