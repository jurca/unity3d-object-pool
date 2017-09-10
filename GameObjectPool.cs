using System;
using UnityEngine;

namespace ObjectPool
{
	[Serializable]
	public class GameObjectPool : ObjectPool<GameObject>
	{
		public static bool IsPooled (GameObject gameObject)
		{
			return gameObject.GetComponent<PooledGameObjectController> ();
		}

		public static void ReturnObjectToPool (GameObject usedObject)
		{
			usedObject.GetComponent<PooledGameObjectController> ().ReturnToPool ();
		}

		protected override GameObject CreateObjectInstance ()
		{
			GameObject objectInstance = GameObject.Instantiate (template);
			PooledGameObjectController controller = objectInstance.GetComponent<PooledGameObjectController> ();
			controller.parentPool = this;
			return objectInstance;
		}

		protected override void ActivateObject (GameObject objectInstance)
		{
			objectInstance.SetActive (true);
		}

		protected override void DeactivateObject (GameObject usedObject)
		{
			usedObject.SetActive (false);
		}
	}
}
