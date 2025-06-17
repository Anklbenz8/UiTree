using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace UIRecycleTreeNamespace {
	internal class TreeEditorWindow : EditorWindow {
		private const float SPLIT_WIDTH = 2;
		private const int WINDOW_WIDTH = 600;
		private const int WINDOW_HEIGHT = 400;

		[SerializeField] TreeViewState treeViewState;
		private Rect bottomToolbarRect => new (0, position.height - 18f, position.width, 18f);

		private static UIRecycleTree _tree;
		private Rect _cursorChangeRect;
		private EditorTree _editorTree;
		private float _currentScrollViewWidth;
		private bool _resize;

		public static void Open(UIRecycleTree tree) {
			_tree = tree;
			var window = GetWindow<TreeEditorWindow>();
			window.titleContent = new GUIContent($"{_tree.name}");
			window.Show();
			window.CreateGUI();
		}

		private void CreateGUI() {
			if (_tree == null) return;
			// Maybe Add something like tree changed
		//	if (treeViewState == null)
				treeViewState = new TreeViewState();
			_editorTree = new EditorTree(treeViewState, _tree);
		}

		private void OnEnable() {
			int x = (Screen.currentResolution.width - WINDOW_WIDTH) / 2;
			int y = (Screen.currentResolution.height - WINDOW_HEIGHT) / 2;
			if(this == null) return;
			position = new Rect(x, y, WINDOW_WIDTH, WINDOW_HEIGHT);
			_currentScrollViewWidth = position.width * 2 / 3;
			_cursorChangeRect = new Rect(_currentScrollViewWidth, 0, SPLIT_WIDTH, position.height);
		}

		private void OnSelectionChange() {
			var selectionList = Selection.instanceIDs;
			if (selectionList.Length > 1 || selectionList.Length == 0) return;
			var gameObject	= GetGameObjectById(selectionList[0]);
			if(gameObject == null) return;

			if (gameObject.TryGetComponent<UIRecycleTree>(out var tree))
				Open(tree);
			else
				_tree = null;

			CreateGUI();
			Repaint();
		}

		private GameObject GetGameObjectById(int instanceID) {
			var unityObject = EditorUtility.InstanceIDToObject(instanceID);
			try {
				return (GameObject)unityObject;
			}
			catch {
				return null;
			}
		}

		private void OnGUI() {
			if (_tree == null)
				return;

			_cursorChangeRect.height = position.height - bottomToolbarRect.height;

			GUILayout.BeginHorizontal();
			GUILayout.BeginScrollView(Vector2.zero, GUILayout.Width(_currentScrollViewWidth));

			DrawTree();
			GUILayout.EndScrollView();

			GUILayout.FlexibleSpace();
			ResizeScrollView();

			DrawSettings();

			GUILayout.EndHorizontal();
			DrawBottom(bottomToolbarRect);

			Repaint();
		}

		private void DrawTree() {
			var labelRect = new Rect(5, 3, 100, 20);
			EditorGUI.LabelField(labelRect, "Tree Hierarchy:", EditorStyles.boldLabel);

			Rect rect = GUILayoutUtility.GetRect(0, 100, 0, 100000);
			rect.y = 25;
			_editorTree.OnGUI(rect);
		}

		private void DrawSettings() {

			float fieldHeight = 20;
			float spacing = 1;
			float leftIndent = 10;
			float yTrackedPos = spacing;

			var labelRect = new Rect(_cursorChangeRect.x + leftIndent, yTrackedPos, position.width - _cursorChangeRect.x, fieldHeight + 5);
			yTrackedPos += fieldHeight + spacing;
			var idRect = new Rect(_cursorChangeRect.x + leftIndent, yTrackedPos, position.width - _cursorChangeRect.x - leftIndent - 5, fieldHeight);
			yTrackedPos += fieldHeight + spacing;
			var namePropertyRect = new Rect(_cursorChangeRect.x + leftIndent, yTrackedPos, position.width - _cursorChangeRect.x - leftIndent - 5, fieldHeight);

			yTrackedPos += fieldHeight + spacing;
			var stylesLabelRect = new Rect(_cursorChangeRect.x + leftIndent, yTrackedPos, 81, fieldHeight);
			var stylesRect = new Rect(stylesLabelRect.max.x + leftIndent, yTrackedPos, position.width - stylesLabelRect.max.x - leftIndent - 5, fieldHeight);

			yTrackedPos += fieldHeight + spacing;
			var isExpandedRect = new Rect(_cursorChangeRect.x + leftIndent, yTrackedPos, position.width - _cursorChangeRect.x - leftIndent - 5, fieldHeight);
			yTrackedPos += fieldHeight + spacing;
			var isCheckedRect = new Rect(_cursorChangeRect.x + leftIndent, yTrackedPos, position.width - _cursorChangeRect.x - leftIndent - 5, fieldHeight);
			//	yTrackedPos += fieldHeight + spacing;
			//	var collectionIndexRect = new Rect(_cursorChangeRect.x + leftIndent, yTrackedPos, position.width - _cursorChangeRect.x - leftIndent - 5, fieldHeight);
			yTrackedPos += fieldHeight + spacing;
			var childCountRect = new Rect(_cursorChangeRect.x + leftIndent, yTrackedPos, position.width - _cursorChangeRect.x - leftIndent - 5, fieldHeight);
			yTrackedPos += fieldHeight + spacing;
			var fullPathRect = new Rect(_cursorChangeRect.x + leftIndent, yTrackedPos, position.width - _cursorChangeRect.x - leftIndent - 5, fieldHeight * 3);

			var isCheckboxesEnabled = _tree.isCheckboxesEnabled;
			var hasSelectedNode = _editorTree.selected != null;
			var nodeName = hasSelectedNode ? _editorTree.selected.name : "-";
			var isExpanded = hasSelectedNode && _editorTree.selected.isExpanded;
			var isChecked = hasSelectedNode && _editorTree.selected.isChecked;
			var childCount = hasSelectedNode ? _editorTree.selected.childCount.ToString() : string.Empty;
			//	var indexInCollectionCount = hasSelectedNode ? _editorTree.selected.indexInCollection.ToString() : string.Empty;
			var styleIndex = hasSelectedNode ? _editorTree.selected.styleIndex.ToString() : string.Empty;
			var nodeId = hasSelectedNode ? _editorTree.selected.nodeId.ToString() : string.Empty;
			var fullPath = hasSelectedNode ? _editorTree.selected.fullPath : string.Empty;

			EditorGUI.LabelField(labelRect, $"Node Properties: ", EditorStyles.boldLabel);
			float originalValue = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 90;

			EditorGUI.BeginDisabledGroup(!hasSelectedNode);

			EditorGUI.LabelField(idRect, "Id:", nodeId, EditorStyles.boldLabel);

			EditorGUI.BeginChangeCheck();
			nodeName = EditorGUI.TextField(namePropertyRect, "Name:", nodeName);
			if (EditorGUI.EndChangeCheck())
				_editorTree.selected.name = nodeName;

			DrawDropdown(stylesLabelRect, stylesRect, new GUIContent(styleIndex));

			EditorGUI.BeginChangeCheck();
			isExpanded = EditorGUI.Toggle(isExpandedRect, "IsExpanded:", isExpanded);
			if (EditorGUI.EndChangeCheck())
				_editorTree.selected.isExpanded = isExpanded;

			EditorGUI.BeginChangeCheck();
			EditorGUI.BeginDisabledGroup(!isCheckboxesEnabled);
			isChecked = EditorGUI.Toggle(isCheckedRect, "IsChecked:", isChecked);
			EditorGUI.EndDisabledGroup();
			if (EditorGUI.EndChangeCheck())
				_editorTree.selected.isChecked = isChecked;

			EditorGUI.LabelField(childCountRect, "Child Count:", childCount);
			EditorGUI.LabelField(fullPathRect, "Full path:", fullPath, EditorStyles.wordWrappedMiniLabel);

			EditorGUI.EndDisabledGroup();

			/*if (EditorGUI.EndChangeCheck()) {
				_editorTree.selected.name = nodeName;
				

				if (isCheckboxesEnabled)
					_editorTree.selected.isChecked = isChecked;*/

			_editorTree.Reload();
			EditorUtility.SetDirty(_tree);

			EditorGUIUtility.labelWidth = originalValue;
		}

		private void DrawBottom(Rect rect) {
			GUILayout.BeginHorizontal();

			//	var style = "miniButton";
			if (GUILayout.Button("Add Root")) {
				_tree.nodes.AddFluent("node");
				_editorTree.Reload();

				Repaint();
			}

			if (GUILayout.Button("Add Child")) {
				_editorTree.selected.nodes.AddFluent("node");
				_editorTree.Reload();

				Repaint();
			}

			if (GUILayout.Button("Remove")) {
				_editorTree.selected.RemoveYourself();
				_editorTree.Reload();

				Repaint();
			}

			if (GUILayout.Button("Clear")) {
				_tree.Clear();
		//		_editorTree.
				_editorTree.Reload();
				
				Repaint();
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void ResizeScrollView() {
			GUI.DrawTexture(_cursorChangeRect, EditorGUIUtility.whiteTexture);
			EditorGUIUtility.AddCursorRect(_cursorChangeRect, MouseCursor.ResizeHorizontal);

			if (Event.current.type == EventType.MouseDown && _cursorChangeRect.Contains(Event.current.mousePosition)) {
				_resize = true;
			}
			if (_resize) {
				_currentScrollViewWidth = Event.current.mousePosition.x;
				_cursorChangeRect.Set(_currentScrollViewWidth, _cursorChangeRect.y, _cursorChangeRect.width, _cursorChangeRect.height);
			}
			if (Event.current.type == EventType.MouseUp)
				_resize = false;
		}

		private void DrawDropdown(Rect labelRect, Rect rect, GUIContent label) {
			EditorGUI.LabelField(labelRect, "Style Index:");
			if (!EditorGUI.DropdownButton(rect, label, FocusType.Passive))
				return;
			GenericMenu menu = new GenericMenu();

			for (var i = 0; i < _tree.nodeStyles.Length; i++)
				menu.AddItem(new GUIContent(i.ToString()), false, HandleItemClicked, i);
			menu.DropDown(rect);
		}
		private void HandleItemClicked(object parameter) {
			if (_editorTree.selected.styleIndex == (int)parameter)
				return;
			_editorTree.selected.styleIndex = (int)parameter;
			EditorUtility.SetDirty(_tree);
		}
	}
}