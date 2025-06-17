using TMPro;
using UnityEngine;

namespace UIRecycleTreeNamespace {
	public class TextControl : MonoBehaviour {
		[SerializeField] private TMP_Text textField;
		public string text {
			get => textField.text;
			set => textField.text = value;
		}
		public NodeTextStyle style {
			set {
				textField.fontSize = value.fontSize;
				textField.font = value.fontAsset;
				textField.fontStyle = value.fontStyle;
				textField.color = value.color;
				textField.wordSpacing = value.wordSpacing;
			}
		}
	}
}