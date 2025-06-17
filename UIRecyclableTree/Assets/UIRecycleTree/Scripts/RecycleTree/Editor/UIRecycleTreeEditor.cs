using UnityEditor;
using UnityEngine;

namespace UIRecycleTreeNamespace {
	[CustomEditor(typeof(UIRecycleTree))]
	public class UIRecycleTreeEditor : ExtendedScrollRectEditor {
		private SerializedProperty _itemHeight;
		private SerializedProperty _layoutGroup;
		private SerializedProperty _nodeStyle;
		private SerializedProperty _pathSeparator;
		private SerializedProperty _onNodeSelected, _onNodeDeselected, _onNodeChecked, _onNodeDblClick, _onNodeExpandStateChanged, _onSelectionChanged;
		private SerializedProperty _fullRowNodes, _highlightSubSelected;
		private SerializedProperty _childIndent, _componentSpacing, _leftPadding, _rightPadding;
		private SerializedProperty _toggleWidth, _toggleIconSize;
		private SerializedProperty _iconEnabled, _iconWidth, _iconSize;
		private SerializedProperty _checkboxEnabled, _recursiveChecked, _checkedWidth, _checkedIconSize;
		
		//--
		private SerializedProperty _expandOnSelect;
		private SerializedProperty _hideExpandToggleWhenHasNoChildren;
		//--

		private UIRecycleTree _tree;
		private int _left, _right, _bottom, _top;
		private bool _paddingFoldout, _nodeComponentPaddingFoldout;

		protected override void OnEnable() {
			base.OnEnable();
			_tree = (UIRecycleTree)target;

			_highlightSubSelected = serializedObject.FindProperty("highlightSubSelected");
			_itemHeight = serializedObject.FindProperty("itemHeight");
			_layoutGroup = serializedObject.FindProperty("contentLayoutGroup");
			_nodeStyle = serializedObject.FindProperty("nodeStylesArray");
			_pathSeparator = serializedObject.FindProperty("pathSeparator");

			_onNodeSelected = serializedObject.FindProperty("onNodeSelected");
			_onNodeDeselected = serializedObject.FindProperty("onNodeDeselected");
			_onNodeChecked = serializedObject.FindProperty("onNodeCheckedChanged");
			_onNodeDblClick = serializedObject.FindProperty("onNodeDblClick");
			_onNodeExpandStateChanged = serializedObject.FindProperty("onNodeExpandStateChanged");
			_onSelectionChanged = serializedObject.FindProperty("onSelectionChanged");

			_fullRowNodes = serializedObject.FindProperty("fullRowNodes");
			_childIndent = serializedObject.FindProperty("childIndent");

			_toggleWidth = serializedObject.FindProperty("toggleWidth");
			_toggleIconSize = serializedObject.FindProperty("toggleIconSize");

			_iconEnabled = serializedObject.FindProperty("imageEnabled");
			_iconWidth = serializedObject.FindProperty("imageWidth");
			_iconSize = serializedObject.FindProperty("imageIconSize");

			_checkboxEnabled = serializedObject.FindProperty("checkboxEnabled");
			_checkedWidth = serializedObject.FindProperty("checkboxWidth");
			_checkedIconSize = serializedObject.FindProperty("checkboxIconSize");
			_recursiveChecked = serializedObject.FindProperty("recursiveChecked");

			_componentSpacing = serializedObject.FindProperty("contentSpacing");
			_leftPadding = serializedObject.FindProperty("leftPadding");
			_rightPadding = serializedObject.FindProperty("rightPadding");
			
			_expandOnSelect = serializedObject.FindProperty("expandOnSelect");
			_hideExpandToggleWhenHasNoChildren = serializedObject.FindProperty("hideExpandToggleWhenHasNoChildren");
		}

		public override void OnInspectorGUI() {
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_fullRowNodes);
			EditorGUILayout.PropertyField(_highlightSubSelected);
			EditorGUILayout.PropertyField(_pathSeparator);
			EditorGUILayout.PropertyField(_expandOnSelect);

			SetPadding();

			_tree.spacing = EditorGUILayout.FloatField("Nodes Spacing", _tree.spacing);


			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("Node Settings:", EditorStyles.boldLabel);

			EditorGUI.indentLevel++;

			_itemHeight.floatValue = EditorGUILayout.FloatField("Node Height", _itemHeight.floatValue);
			EditorGUILayout.PropertyField(_childIndent);
			EditorGUILayout.Space(3);
			DrawNodeComponentPadding();
			EditorGUILayout.Space(3);
			DrawExpandToggle();
			DrawCheckbox();
			DrawIcon();

			EditorGUI.indentLevel--;
			EditorGUILayout.Space(5);
			EditorGUILayout.PropertyField(_nodeStyle);

			EditorGUILayout.Space(5);

			EditorGUILayout.LabelField("Tree Constructor:", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			if (GUILayout.Button("Open Tree Constructor"))
				TreeEditorWindow.Open(_tree);
			EditorGUI.indentLevel--;
			EditorGUILayout.Space(10);

			EditorGUILayout.LabelField("Scroll rect:", EditorStyles.boldLabel);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(_layoutGroup);
			serializedObject.ApplyModifiedProperties();

			base.OnInspectorGUI();

			EditorGUILayout.PropertyField(_onNodeSelected);
			EditorGUILayout.PropertyField(_onNodeDeselected);
			EditorGUILayout.PropertyField(_onNodeChecked);
			EditorGUILayout.PropertyField(_onNodeDblClick);
			EditorGUILayout.PropertyField(_onNodeExpandStateChanged);
			EditorGUILayout.PropertyField(_onSelectionChanged);
			serializedObject.ApplyModifiedProperties();
			if (EditorGUI.EndChangeCheck())
				EditorUtility.SetDirty(_tree);
		}
		private void DrawNodeComponentPadding() {
			_nodeComponentPaddingFoldout = EditorGUILayout.Foldout(_nodeComponentPaddingFoldout, "Node Content Padding", true);
			if (_nodeComponentPaddingFoldout) {
				EditorGUI.indentLevel++;
				_leftPadding.floatValue = EditorGUILayout.FloatField("Left", _leftPadding.floatValue);
				_rightPadding.floatValue = EditorGUILayout.FloatField("Right", _rightPadding.floatValue);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.PropertyField(_componentSpacing);
		}
		private void DrawCheckbox() {
			EditorGUI.indentLevel--;
			_checkboxEnabled.boolValue = EditorGUILayout.BeginToggleGroup("Checkbox Enabled", _checkboxEnabled.boolValue);

			EditorGUI.indentLevel++;
			EditorGUI.indentLevel++;
			if (_checkboxEnabled.boolValue) {
				EditorGUILayout.PropertyField(_recursiveChecked);
				EditorGUILayout.PropertyField(_checkedWidth);
				EditorGUILayout.PropertyField(_checkedIconSize);
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.EndToggleGroup();
		}
		private void DrawIcon() {

			EditorGUI.indentLevel--;
			_iconEnabled.boolValue = EditorGUILayout.BeginToggleGroup("Image Enabled", _iconEnabled.boolValue);
			EditorGUI.indentLevel++;
			EditorGUI.indentLevel++;
			if (_iconEnabled.boolValue) {
				EditorGUILayout.PropertyField(_iconWidth);
				EditorGUILayout.PropertyField(_iconSize);
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.EndToggleGroup();
		}
		private void DrawExpandToggle() {
			EditorGUILayout.LabelField("Expand Toggle", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			
			EditorGUILayout.PropertyField(_hideExpandToggleWhenHasNoChildren);
			EditorGUILayout.PropertyField(_toggleWidth);
			EditorGUILayout.PropertyField(_toggleIconSize);
			EditorGUI.indentLevel--;
		}

		private void SetPadding() {
			_paddingFoldout = EditorGUILayout.Foldout(_paddingFoldout, "Tree Content Padding", true);
			if (!_paddingFoldout) return;

			EditorGUI.indentLevel++;
			_left = EditorGUILayout.IntField("Left", _tree.contentPadding.left);
			_right = EditorGUILayout.IntField("Right", _tree.contentPadding.right);
			EditorGUI.indentLevel--;

			_tree.contentPadding = new RectOffset(_left, _right, 0, 0);
		}
	}
}