using System;
using UnityEngine;
using UnityEngine.UI;
namespace UIRecycleTreeNamespace {

	[Serializable]
	public class Background {
		public Icon backgroundImage = new() {color = Color.clear};
		public Image.Type imageType;
		public float pixelPerUnitMultiplier = 1;
	}
}
