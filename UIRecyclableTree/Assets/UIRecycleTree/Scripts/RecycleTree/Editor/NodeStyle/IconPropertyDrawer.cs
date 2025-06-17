using UnityEditor;
using UnityEngine;
namespace UIRecycleTreeNamespace {

	[CustomPropertyDrawer(typeof(Icon))]
	public class IconPropertyDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			var sprite = property.FindPropertyRelative("sprite");
			var color = property.FindPropertyRelative("color");

			var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
			var miniatureRect = new Rect(labelRect.x + labelRect.width, position.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);

			var fieldWidth = position.width - EditorGUIUtility.labelWidth - miniatureRect.width;
			var textureRect = new Rect(miniatureRect.x + miniatureRect.width + 2, position.y, fieldWidth / 2, EditorGUIUtility.singleLineHeight);
			var colorRect = new Rect(textureRect.x + textureRect.width, position.y, fieldWidth / 2, position.height);

			EditorGUI.LabelField(labelRect, label.text);
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			sprite.objectReferenceValue = EditorGUI.ObjectField(textureRect, sprite.objectReferenceValue, typeof(Sprite), false) as Sprite;
			EditorGUI.DrawRect(miniatureRect, new Color(0.157f, 0.157f, 0.157f, 0.5f));

			if (sprite.objectReferenceValue != null) {
				Texture2D myTexture = AssetPreview.GetAssetPreview(sprite.objectReferenceValue);
				GUI.Label(miniatureRect, myTexture);
			}
			color.colorValue = EditorGUI.ColorField(colorRect, color.colorValue);
			EditorGUI.EndProperty();
			EditorGUI.indentLevel = indent;
		}
	}
}
