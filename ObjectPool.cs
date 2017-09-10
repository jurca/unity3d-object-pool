using System;
using UnityEngine;

namespace ObjectPool
{
	[System.Serializable]
	public abstract class ObjectPool<T>
		where T : class
	{
		public int minCapacity;
		public int maxCapacity;
		public T template;
		public bool returnNullOnUnderflow;
		public string label = "<Unlabeled object pool>";

		private T[] pool;
		private bool ready = false;
		private int queueStart = 0;
		private int queueEnd = 0;
		private int objectsInPool = 0;

		public void Init ()
		{
			if (ready) {
				return;
			}
			ready = true;

			if (template == null) {
				throw new ArgumentNullException ("template", "The template object cannot be null");
			}
			if (minCapacity <= 0) {
				throw new ArgumentOutOfRangeException (
					"minCapacity",
					"The minimum capacity must be a positive integer"
				);
			}
			if (maxCapacity < minCapacity) {
				throw new ArgumentOutOfRangeException (
					"maxCapacity",
					string.Format (
						"The maximum capacity ({0}) cannot be lower than the minimum capacity ({1})",
						maxCapacity,
						minCapacity
					)
				);
			}

			pool = new T[minCapacity];
			for (int i = 0; i < minCapacity; i++) {
				AddObjectInstance ();
			}
		}

		public T GetObject ()
		{
			if (!ready) {
				throw new InvalidOperationException ("Call the Init() method first");
			}

			if (objectsInPool == 0) {
				if (pool.Length < maxCapacity) {
					Grow ();
				} else if (returnNullOnUnderflow) {
					Debug.Log (string.Format (
						"The {0} object pool has underflown and is returning nulls. Make sure the capacity is sufficient and check for memleaks.",
						label
					));
					return null;
				} else {
					throw new OutOfMemoryException (string.Format (
						"The maximum capacity ({0}) of the object pool '{1}' has been exceeded",
						maxCapacity,
						label
					));
				}
			}

			T objectInstance = pool [queueStart];

			ActivateObject (objectInstance);
			queueStart = (queueStart + 1) % pool.Length;
			objectsInPool--;
			return objectInstance;
		}

		public void ReturnObject (T usedObject)
		{
			if (!ready) {
				throw new InvalidOperationException ("Call the Init() method first");
			}

			DeactivateObject (usedObject);
			pool [queueEnd] = usedObject;
			queueEnd = (queueEnd + 1) % pool.Length;
			objectsInPool++;
		}

		protected abstract T CreateObjectInstance ();

		protected abstract void ActivateObject (T pooledObject);

		protected abstract void DeactivateObject (T pooledObject);

		void Grow ()
		{
			Debug.LogWarning (string.Format (
				"The currenct capacity ({0}) of the pool '{1}' has been exceeded, increasing capacity",
				pool.Length,
				label
			));

			int newCapacity = pool.Length > maxCapacity / 2 ? maxCapacity : pool.Length * 2;
			int capacityIncrease = newCapacity - pool.Length;
			pool = new T[newCapacity];

			for (int i = 0; i < capacityIncrease; i++) {
				AddObjectInstance ();
			}
		}

		void AddObjectInstance ()
		{
			T objectInstance = CreateObjectInstance ();
			ReturnObject (objectInstance);
		}
	}
}
