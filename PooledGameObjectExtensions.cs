using UnityEngine;

namespace ObjectPool
{
	public static class PooledGameObjectExtensions
	{
		public static bool IsPooled (this GameObject gameObject)
		{
			return GameObjectPool.IsPooled (gameObject);
		}

		public static void ReturnToPool (this GameObject gameObject)
		{
			GameObjectPool.ReturnObjectToPool (gameObject);
		}
	}
}
