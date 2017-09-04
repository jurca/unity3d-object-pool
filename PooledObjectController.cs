using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObjectController : MonoBehaviour {
	private ObjectPool parentPool;

	public void SetParentPool(ObjectPool newParentPool) {
		if (parentPool != null) {
			throw new InvalidOperationException ("Cannot reconfigure the parent pool reference once it has been set");
		}

		parentPool = newParentPool;
	}

	public void ReturnToPool() {
		parentPool.ReturnObject (this.gameObject);
	}
}
