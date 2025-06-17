using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UIRecycleTreeNamespace {
	internal class EditorTree : TreeView {
		private readonly UIRecycleTree _tree;
		private const float ROW_HEIGHT = 18;
		private const float BASE_INDENT = 10;
		private const float TOGGLE_WIDTH = 18;

		public Node selected => _selected;
		private Node _selected;

		public EditorTree(TreeViewState state, UIRecycleTree tree) : base(state) {
			_tree = tree;
		//	extraSpaceBeforeIconAndLabel = 18f;
			rowHeight = ROW_HEIGHT;
			baseIndent = BASE_INDENT;
			showBorder = true;

			Reload();
		}

		protected override TreeViewItem BuildRoot() =>
				new TreeViewItem {id = 0, depth = -1};

		protected override bool CanMultiSelect(TreeViewItem item) =>
				false;
		protected override bool CanStartDrag(CanStartDragArgs args) => false;

		protected override IList<TreeViewItem> BuildRows(TreeViewItem root) {
			var rows = GetRows() ?? new List<TreeViewItem>(200);
			if (_tree == null) return null;
			rows.Clear();
			foreach (var node in _tree.nodes) {
				var item = CreateEditorTreeItem(node);
				root.AddChild(item);
				rows.Add(item);
				if (node.hasChildren) {
					if (IsExpanded(item.id)) {
						AddChildrenRecursive(node, item, rows);
					}
					else {
						item.children = CreateChildListForCollapsedParent();
					}
				}
			}
			SetupDepthsFromParentsAndChildren(root);
			return rows;
		}

		private void AddChildrenRecursive(Node node, TreeViewItem item, IList<TreeViewItem> rows) {
			int childCount = node.childCount;

			item.children = new List<TreeViewItem>(childCount);
			for (int i = 0; i < childCount; ++i) {
				var childNode = node.nodes[i];
				var childItem = CreateEditorTreeItem(childNode);
				item.AddChild(childItem);
				rows.Add(childItem);

				if (childNode.hasChildren) {
					if (IsExpanded(childItem.id)) {
						AddChildrenRecursive(childNode, childItem, rows);
					}
					else {
						childItem.children = CreateChildListForCollapsedParent();
					}
				}
			}
		}

		static TreeViewItem CreateEditorTreeItem(Node node) =>
				new TreeViewItem(node.nodeId, -1, node.name);

		protected override IList<int> GetAncestors(int id) {
			var node = FindNodeById(id);

			List<int> ancestors = new List<int>();
			while (node.parentNode != null) {
				ancestors.Add(node.parentNode.nodeId);
				node = node.parentNode;
			}

			return ancestors;
		}

		protected override IList<int> GetDescendantsThatHaveChildren(int id) {
			Stack<Node> stack = new();

			var start = FindNodeById(id);
			stack.Push(start);

			var parents = new List<int>();
			while (stack.Count > 0) {
				var current = stack.Pop();
				parents.Add(current.nodeId);
				for (int i = 0; i < current.childCount; ++i) {
					if (current.childCount > 0)
						stack.Push(current.nodes[i]);
				}
			}

			return parents;
		}

		private Node FindNodeById(int id) =>
				_tree.FindNodeByIdRecursive(id);

		// Custom GUI

		protected override void RowGUI(RowGUIArgs args) {
		    var checkboxesEnabled = _tree.isCheckboxesEnabled;

			var node = FindNodeById(args.item.id);
			if (node == null)
				return;

			Rect toggleRect = args.rowRect;
			toggleRect.x += GetContentIndent(args.item);
			toggleRect.width = 16f;

			extraSpaceBeforeIconAndLabel = checkboxesEnabled? TOGGLE_WIDTH:0;
			if (checkboxesEnabled) {
				EditorGUI.BeginChangeCheck();
				bool isChecked = EditorGUI.Toggle(toggleRect, node.isChecked);
				if (EditorGUI.EndChangeCheck())
					node.isChecked = isChecked;
			}

			base.RowGUI(args);
		}

		// Selection
		protected override void SelectionChanged(IList<int> selectedIds) =>
				_selected = FindNodeById(selectedIds[0]);

		// Reordering

		protected override void SetupDragAndDrop(SetupDragAndDropArgs args) {
			DragAndDrop.PrepareStartDrag();

			var sortedDraggedIDs = SortItemIDsInRowOrder(args.draggedItemIDs);

			List<UnityObject> objList = new List<UnityObject>(sortedDraggedIDs.Count);
			foreach (var id in sortedDraggedIDs) {
				UnityObject obj = EditorUtility.InstanceIDToObject(id);
				if (obj != null)
					objList.Add(obj);
			}

			DragAndDrop.objectReferences = objList.ToArray();

			string title = objList.Count > 1 ? "<Multiple>" : objList[0].name;
			DragAndDrop.StartDrag(title);
		}

		private int GetAdjustedInsertIndex(Transform parent, Transform transformToInsert, int insertIndex) {
			if (transformToInsert.parent == parent && transformToInsert.GetSiblingIndex() < insertIndex)
				return --insertIndex;
			return insertIndex;
		}

		private bool IsValidReparenting(Transform parent, List<Transform> transformsToMove) {
			if (parent == null)
				return true;

			foreach (var transformToMove in transformsToMove) {
				if (transformToMove == parent)
					return false;

				if (IsHoveredAChildOfDragged(parent, transformToMove))
					return false;
			}

			return true;
		}
		private bool IsHoveredAChildOfDragged(Transform hovered, Transform dragged) {
			Transform t = hovered.parent;
			while (t) {
				if (t == dragged)
					return true;
				t = t.parent;
			}
			return false;
		}


		// Returns true if there is an ancestor of transform in the transforms list
		static bool IsDescendantOf(Transform transform, List<Transform> transforms) {
			while (transform != null) {
				transform = transform.parent;
				if (transforms.Contains(transform))
					return true;
			}
			return false;
		}

		static void RemoveItemsThatAreDescendantsFromOtherItems(List<Transform> transforms) {
			transforms.RemoveAll(t => IsDescendantOf(t, transforms));
		}

		protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) {
			// First check if the dragged objects are GameObjects
			/*
			var draggedObjects = DragAndDrop.objectReferences;
			var transforms = new List<Transform>(draggedObjects.Length);
			foreach (var obj in draggedObjects) {
				var go = obj as GameObject;
				if (go == null) {
					return DragAndDropVisualMode.None;
				}

				transforms.Add(go.transform);
			}

			// Filter out any unnecessary transforms before the reparent operation
			RemoveItemsThatAreDescendantsFromOtherItems(transforms);

			// Reparent
			if (args.performDrop) {
				switch (args.dragAndDropPosition) {
					case DragAndDropPosition.UponItem:
					case DragAndDropPosition.BetweenItems:
						Transform parent = args.parentItem != null ? FindNodeById(args.parentItem.id).transform : null;

						if (!IsValidReparenting(parent, transforms))
							return DragAndDropVisualMode.None;

						foreach (var trans in transforms)
							trans.SetParent(parent);

						if (args.dragAndDropPosition == DragAndDropPosition.BetweenItems) {
							int insertIndex = args.insertAtIndex;
							for (int i = transforms.Count - 1; i >= 0; i--) {
								var transform = transforms[i];
								insertIndex = GetAdjustedInsertIndex(parent, transform, insertIndex);
								transform.SetSiblingIndex(insertIndex);
							}
						}
						break;

					case DragAndDropPosition.OutsideItems:
						foreach (var trans in transforms) {
							trans.SetParent(null); // make root when dragged to empty space in treeview
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				Reload();
				SetSelection(transforms.Select(t => t.gameObject.GetInstanceID()).ToList(), TreeViewSelectionOptions.RevealAndFrame);
			}
			*/

			return DragAndDropVisualMode.None; //Move;
		}
	}
}