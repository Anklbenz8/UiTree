using UnityEditor;

namespace UIRecycleTreeNamespace {
	[CustomEditor(typeof(NodeStyle))]
	public class NodeStyleEditor : Editor {
		private SerializedProperty _toggleIcons, _imageIcons, _checkboxIcons;
		private SerializedProperty _nodeBackground, _selectedStyle, _subSelectedStyle, _fadedAlpha;
		private SerializedProperty _fontAsset, _fontSize, _fontColor, _fontStyle, _fontWordSpacing;
		private bool _subSelectedFoldout;
		protected void OnEnable() {
			_toggleIcons = serializedObject.FindProperty("_toggleIcons");
			_imageIcons = serializedObject.FindProperty("_imageIcons");
			_checkboxIcons = serializedObject.FindProperty("_checkboxIcons");
			
			_selectedStyle = serializedObject.FindProperty("_selectedState");
			_subSelectedStyle = serializedObject.FindProperty("_subSelectedState");
			_fadedAlpha = serializedObject.FindProperty("_fadedAlpha");

			_nodeBackground = serializedObject.FindProperty("_background");
	
			var nodeText = serializedObject.FindProperty("_nodeTextStyle");
			_fontAsset = nodeText.FindPropertyRelative("fontAsset");
			_fontSize = nodeText.FindPropertyRelative("fontSize");
			_fontColor = nodeText.FindPropertyRelative("color");
			_fontStyle = nodeText.FindPropertyRelative("fontStyle");
			_fontWordSpacing = nodeText.FindPropertyRelative("wordSpacing");
		}

		public override void OnInspectorGUI() {
			EditorGUILayout.LabelField("Node Icons:", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_toggleIcons, true);
			EditorGUILayout.PropertyField(_imageIcons, true);
			EditorGUILayout.PropertyField(_checkboxIcons, true);
			EditorGUI.indentLevel--;
			
			EditorGUILayout.LabelField("Node Regular State Style:", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_nodeBackground);
            EditorGUILayout.Space(3);
			EditorGUILayout.PropertyField(_fontAsset);
			EditorGUILayout.PropertyField(_fontSize);
			EditorGUILayout.PropertyField(_fontColor);
			EditorGUILayout.PropertyField(_fontStyle);
			EditorGUILayout.PropertyField(_fontWordSpacing);
			EditorGUI.indentLevel--;

			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("Node Selection States Styles:", EditorStyles.boldLabel);

			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_selectedStyle);
			EditorGUILayout.PropertyField(_subSelectedStyle);
			EditorGUI.indentLevel--;

			EditorGUILayout.Space(5);

			EditorGUILayout.LabelField("Faded state", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_fadedAlpha);
			EditorGUI.indentLevel--;
			serializedObject.ApplyModifiedProperties();
		}
	}
}