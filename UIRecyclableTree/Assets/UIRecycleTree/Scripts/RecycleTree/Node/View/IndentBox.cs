using UnityEngine;

namespace UIRecycleTreeNamespace {
	public class IndentBox : MonoBehaviour {
		private RectTransform _rectTransform;
		public float indent {
			set {
				var rect = (RectTransform)transform;
				rect.sizeDelta = new Vector2(value, rect.sizeDelta.y);
			}
		}
		private void Awake() =>
			_rectTransform = (RectTransform)transform;
	}
}
