using UnityEditor;
using UnityEngine;
namespace UIRecycleTreeNamespace {

	[CustomPropertyDrawer(typeof(Background))]
	public class BackgroundPropertyDrawer : PropertyDrawer {
		private readonly string[] _options = new string[2] {"Simple", "Sliced"};
		private float _fieldHeight;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var singleLineHeight = EditorGUIUtility.singleLineHeight;
			_fieldHeight = singleLineHeight;
			var nodeBackground = property.FindPropertyRelative("backgroundImage");
			var imageType = property.FindPropertyRelative("imageType");
			var pixelPerUnitMultiplier = property.FindPropertyRelative("pixelPerUnitMultiplier");

			EditorGUI.BeginProperty(position, label, property);

			var backgroundRect = new Rect(position.x, position.y, position.width, singleLineHeight);
			EditorGUI.PropertyField(backgroundRect, nodeBackground);

			var nodeSprite = nodeBackground.FindPropertyRelative("sprite");
			if (nodeSprite.objectReferenceValue != null) {
				EditorGUI.indentLevel++;

				_fieldHeight += singleLineHeight;
				var imageTypeRect = new Rect(backgroundRect.x, backgroundRect.max.y, backgroundRect.width, singleLineHeight);
				imageType.enumValueIndex = EditorGUI.Popup(imageTypeRect, "ImageType", imageType.enumValueIndex, _options);

				if (imageType.enumValueIndex == 1) {
					EditorGUI.indentLevel++;

					_fieldHeight += singleLineHeight;
					var pixelPerUnitRect = new Rect(backgroundRect.x, imageTypeRect.max.y, backgroundRect.width, singleLineHeight);
					EditorGUI.PropertyField(pixelPerUnitRect, pixelPerUnitMultiplier);

					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}
			EditorGUI.EndProperty();
		}
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => _fieldHeight;
	}
}
