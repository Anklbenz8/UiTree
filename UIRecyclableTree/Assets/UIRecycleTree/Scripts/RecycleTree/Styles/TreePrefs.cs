using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace UIRecycleTreeNamespace {
	public class TreePrefs {
		public bool fullRectSelect { get; set; }
		public float childIndent { get; set; }
		public float toggleWidth { get; set; }
		public Vector2 toggleIconSize { get; set; }
		public bool iconEnabled { get; set; }
		public float iconWidth { get; set; }
		public Vector2 iconSize { get; set; }
		public bool checkboxEnabled { get; set; }
		public float checkedWidth { get; set; }
		public Vector2 checkedIconSize { get; set; }
		public float leftPadding { get; set; }
		public float rightPadding { get; set; }
		public float spacing { get; set; }
		//-- UnderTest
		public bool hideExpandToggleWhenHasNoChildren { get; set; }
		//
	}
}
