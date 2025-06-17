using UnityEditor;
using UnityEngine;
namespace UIRecycleTreeNamespace {

	[CustomPropertyDrawer(typeof(StateStyle))]
	public class StateStylePropertyDrawer : PropertyDrawer {
		private float _fieldHeight;
		private const float FIELD_SPACING = 2;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var singleLineHeight = EditorGUIUtility.singleLineHeight;
			_fieldHeight = singleLineHeight;

			var foldoutRect = new Rect(position.x, position.y, position.width, singleLineHeight);
			property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
			if (!property.isExpanded) return;

			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.indentLevel++;
			var background = property.FindPropertyRelative("background");
			var backgroundPropertyHeight = EditorGUI.GetPropertyHeight(background);
			_fieldHeight += backgroundPropertyHeight;
			var backgroundRect = new Rect(foldoutRect.x, foldoutRect.max.y, position.width, backgroundPropertyHeight);
			EditorGUI.PropertyField(backgroundRect, background);

			_fieldHeight += singleLineHeight;
			var overrideFont = property.FindPropertyRelative("overrideFont");
			var overrideFontRect = new Rect(backgroundRect.x, backgroundRect.max.y, backgroundRect.width, singleLineHeight);
			overrideFont.boolValue = EditorGUI.ToggleLeft(overrideFontRect, "Override Regular Font Style", overrideFont.boolValue);

			if (overrideFont.boolValue) {
				EditorGUI.indentLevel++;
				var nodeTextStyle = property.FindPropertyRelative("textStyle");
				var font = nodeTextStyle.FindPropertyRelative("fontAsset");
				var fontSize = nodeTextStyle.FindPropertyRelative("fontSize");
				var fontColor = nodeTextStyle.FindPropertyRelative("color");
				var fontStyle = nodeTextStyle.FindPropertyRelative("fontStyle");
				var fontWordSpacing = nodeTextStyle.FindPropertyRelative("wordSpacing");

				_fieldHeight += singleLineHeight;
				var fontAssetRect = new Rect(overrideFontRect.x, overrideFontRect.max.y, backgroundRect.width, singleLineHeight);
				EditorGUI.PropertyField(fontAssetRect, font);

				_fieldHeight += EditorGUIUtility.singleLineHeight;
				var fontSizeRect = new Rect(overrideFontRect.x, fontAssetRect.max.y + FIELD_SPACING, backgroundRect.width, singleLineHeight);
				EditorGUI.PropertyField(fontSizeRect, fontSize);

				_fieldHeight += singleLineHeight + FIELD_SPACING;
				var fontColorRect = new Rect(overrideFontRect.x, fontSizeRect.max.y + FIELD_SPACING, backgroundRect.width, singleLineHeight);
				EditorGUI.PropertyField(fontColorRect, fontColor);

				_fieldHeight += singleLineHeight + FIELD_SPACING;
				var fontStyleRect = new Rect(overrideFontRect.x, fontColorRect.max.y + FIELD_SPACING, backgroundRect.width, singleLineHeight);
				EditorGUI.PropertyField(fontStyleRect, fontStyle);

				_fieldHeight += singleLineHeight + FIELD_SPACING;
				var fontWordSpaceRect = new Rect(overrideFontRect.x, fontStyleRect.max.y + FIELD_SPACING, backgroundRect.width, singleLineHeight);
				EditorGUI.PropertyField(fontWordSpaceRect, fontWordSpacing);

				EditorGUI.indentLevel--;
			}

			EditorGUI.indentLevel--;

			EditorGUI.EndProperty();
		}
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => _fieldHeight;
	}
}
