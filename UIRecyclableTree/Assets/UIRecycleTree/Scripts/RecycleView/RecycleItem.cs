using UnityEngine;
namespace UIRecycleTreeNamespace {

	public abstract class RecycleItem : MonoBehaviour {
		public abstract RectTransform rectTransform { get; }
	}
}
