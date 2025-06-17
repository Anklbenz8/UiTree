using UnityEditor;
using UnityEngine;

namespace UIRecycleTreeNamespace {
	[CustomEditor(typeof(RecycleView))]
	public class RecycleViewEditor : ExtendedScrollRectEditor {
		private SerializedProperty _template;
		private SerializedProperty _itemHeight;
		private SerializedProperty _layoutGroup;
		private RecycleView _recycleView;
		private int _left, _right, _bottom, _top;
		private bool _paddingFoldout;

		protected override void OnEnable() {
			base.OnEnable();
			_recycleView = (RecycleView)target;
			_template = serializedObject.FindProperty("template");
			_itemHeight = serializedObject.FindProperty("itemHeight");
			_layoutGroup = serializedObject.FindProperty("contentLayoutGroup");
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			EditorGUILayout.PropertyField(_template);
			EditorGUILayout.PropertyField(_layoutGroup);
			EditorGUILayout.PropertyField(_itemHeight);
			serializedObject.ApplyModifiedProperties();

			if (_layoutGroup.objectReferenceValue == null) return;
			SetPadding();
			SetSpacing();

			serializedObject.ApplyModifiedProperties();
		}
		
		private void SetSpacing() =>
				_recycleView.spacing = EditorGUILayout.FloatField("Spacing", _recycleView.spacing);

		private void SetPadding() {
			_paddingFoldout = EditorGUILayout.Foldout(_paddingFoldout, "Padding", true);
			if (!_paddingFoldout) return;

			EditorGUI.indentLevel++;
			_left = EditorGUILayout.IntField("Left", _recycleView.contentPadding.left);
			_right = EditorGUILayout.IntField("Right", _recycleView.contentPadding.right);
			_top = EditorGUILayout.IntField("Top", _recycleView.contentPadding.top);
			_bottom = EditorGUILayout.IntField("Bottom", _recycleView.contentPadding.bottom);
			EditorGUI.indentLevel--;

			_recycleView.contentPadding = new RectOffset(_left, _right, _top, _bottom);
		}
	}
}