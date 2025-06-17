using System;
using TMPro;
using UnityEngine;

namespace UIRecycleTreeNamespace {
	[Serializable]
	public class NodeTextStyle {
		public TMP_FontAsset fontAsset;
		public float fontSize = 25f;
		public Color color = Color.white;
		public TMPro.FontStyles fontStyle;
		public float wordSpacing;
	}
}