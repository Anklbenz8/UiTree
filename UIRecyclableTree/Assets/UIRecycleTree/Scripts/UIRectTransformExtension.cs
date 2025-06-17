using UnityEngine;
namespace UIRecycleTreeNamespace{

	public static class UIRectTransformExtension {
		public static Vector3[] GetWorldCorners(this RectTransform rectTransform) {
			Vector3[] corners = new Vector3[4];
			rectTransform.GetWorldCorners(corners);
			return corners;
		}
		public static float MaxY(this RectTransform rectTransform) {
			var corners = rectTransform.GetWorldCorners();
			return corners[1].y;
		}

		public static float MinY(this RectTransform rectTransform) {
			var corners = rectTransform.GetWorldCorners();
			return corners[0].y;
		}

		public static float MaxX(this RectTransform rectTransform) {
			var corners = rectTransform.GetWorldCorners();
			return corners[2].x;
		}

		public static float MinX(this RectTransform rectTransform) {
			var corners = rectTransform.GetWorldCorners();
			return corners[0].x;
		}
	}
}