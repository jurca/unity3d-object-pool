using UnityEngine;

namespace ObjectPool
{
	public class PooledGameObjectController : MonoBehaviour
	{
		[HideInInspector] public GameObjectPool parentPool;

		public void ReturnToPool ()
		{
			parentPool.ReturnObject (this.gameObject);
		}
	}
}
