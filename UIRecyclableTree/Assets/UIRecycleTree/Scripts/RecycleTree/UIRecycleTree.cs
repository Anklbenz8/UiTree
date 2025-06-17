using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace UIRecycleTreeNamespace {
	[Serializable]
	public class NodeEvent : UnityEvent<Node> {
	}
	[Serializable]
	public class NodeCountEvent : UnityEvent<int> {
	}

	public class UIRecycleTree : RecycleView, IRecycleDataSource {
		private const string ITEM_RESOURCE_NAME = "UINodeView_template";
		public NodeEvent onNodeSelected = new();
		public NodeEvent onNodeDeselected = new();
		public NodeEvent onNodeCheckedChanged = new();
		public NodeEvent onNodeClick = new();
		public NodeEvent onNodeDblClick = new();
		public NodeEvent onNodeExpandStateChanged = new();
		public NodeEvent onSelectionChanged = new();
		public NodeCountEvent onVisibleNodesCountChanged = new();

		[SerializeField] private bool fullRowNodes;
		[SerializeField] private bool highlightSubSelected;

		[SerializeField] private float childIndent = 55;

		[SerializeField] private float leftPadding = 1;
		[SerializeField] private float rightPadding = 1;
		[SerializeField] private float contentSpacing = 1;

		[SerializeField] private float toggleWidth = 40;
		[SerializeField] private Vector2 toggleIconSize = new(30, 30);

		[SerializeField] private bool imageEnabled = true;
		[SerializeField] private float imageWidth = 40;
		[SerializeField] private Vector2 imageIconSize = new(35, 35);

		[SerializeField] private bool checkboxEnabled;
		[SerializeField] private bool recursiveChecked;
		[SerializeField] private float checkboxWidth = 40;
		[SerializeField] private Vector2 checkboxIconSize = new(35, 35);

		[SerializeReference] private Node root;
		[SerializeField] private string pathSeparator = "/";
		[SerializeField] private List<Node> expandedNodes;

		[SerializeField] private NodeStyle[] nodeStylesArray;
		[SerializeField] private int lastNodeId;

		[SerializeField] private bool expandOnSelect;
		[SerializeField] private bool hideExpandToggleWhenHasNoChildren;
		public NodeCollection nodes => root.nodes;
		public Node rootNode => root;
		public int nodesCount => root.GetAllChildrenCountRecursive() - 1; //subtract root node
		public bool isCheckboxesEnabled => checkboxEnabled;
		public string separator {
			get => pathSeparator;
			set => pathSeparator = value;
		}
		public NodeStyle[] nodeStyles => nodeStylesArray;
		public int expandedCount => expandedNodes.Count;
		public Node selectedNode => _selectedNode;
		public bool hasSelected => _selectedNode != null;
		public bool isRecursiveChecked {
			get => recursiveChecked;
			set => recursiveChecked = value;
		}

		private Node _selectedNode;
		private float _maxItemWidth;
		private bool _isAwaitReloadForFocusOn;
		private int _focusedNodeIndexInExpandedNodesList;

		public void ExpandAll() {
			foreach (var node in nodes)
				node.ExpandAllWithoutNotify();
			Rebuild();
		}
		public void CollapseAll() {
			foreach (var node in nodes)
				node.CollapseAllWithoutNotify();
			Rebuild();
		}

		public void Clear() {
			nodes.Clear();

			lastNodeId = root.nodeId;
		}
		
		public void UpdateNodeCheckedState(Node node) {
			NodeCheckedStateChangedNotify(node);
			Repaint();
		}

		public void NodeCheckedStateChangedNotify(Node node) =>
				onNodeCheckedChanged?.Invoke(node);

		public void Rebuild() {
			//on some versions of Unity isActiveAndEnable sometimes gives the wrong result, adding additional enabled check solves the problem
			if (!Application.isPlaying || !isActiveAndEnabled || !enabled)
				return;

			expandedNodes = new List<Node>();
			root.GetAllExpandedChildrenRecursive(expandedNodes);
			StartCoroutine(base.Reload());
			onVisibleNodesCountChanged?.Invoke(expandedNodes.Count);
		}
		public void MergeDataWithView(RecycleItem recycleItem, int indexInExpandedNodes) {
			var node = expandedNodes[indexInExpandedNodes];
			var view = (NodeView)recycleItem;

			view.ClearPreviousSubscribes();
			var nodeStyleIndex = node.styleIndex;
			if (nodeStylesArray.Length == 0 || nodeStyleIndex >= nodeStylesArray.Length)
				throw new Exception($"NodeStylesArray is empty or The Node {node.name} has an styleIndex {nodeStyleIndex} that is not in the UIRecycleTree stylesArray.");

			var style = nodeStylesArray[nodeStyleIndex];
			if (style == null)
				throw new Exception($"Tree not contain nodeStyle with index {nodeStyleIndex}. Please add Node Style or change styleIndex in node named {node.name} id {node.nodeId}");

			SetNodeStyle(style, node, view);

			view.text = node.name;
			view.indent = node.depth;
			view.isChecked = node.isChecked;
			view.isFaded = node.isFaded;

			//-- hasChildren
			view.hasChildren = node.hasChildren;
			//--hasChildren

			if (!node.hasChildren)
				view.state = ExpandedState.NoChild;
			else
				view.state = node.isExpanded
						? ExpandedState.Expanded
						: ExpandedState.Collapsed;

			view.ClickedEvent += delegate { OnNodeClicked(node); };
			view.ExpandClickEvent += delegate { OnNodeExpandClicked(node); };
			view.CheckboxClickedEvent += delegate { NodeCheckedClicked(node); };
			view.DoubleClickedEvent += delegate { OnNodeDoubleClick(node); };
			view.Refresh();
		}
		private void SetNodeStyle(NodeStyle style, Node node, NodeView view) {

			view.fadedAlpha = style.fadeAlpha;
			view.toggleIcons = style.toggleIcons;
			view.imageIcons = style.imageIcons;
			view.checkboxIcons = style.checkboxIcons;

			if (node.isSelected) {
				var selectedStyle = style.selectedState;
				view.backgroundStyle = selectedStyle.background;
				view.textStyle = selectedStyle.overrideFont ? selectedStyle.textStyle : style.textStyle;
			}
			else if (node.isSubSelected) {
				var subSelectedStyle = style.subSelectedState;
				view.backgroundStyle = subSelectedStyle.background;
				view.textStyle = subSelectedStyle.overrideFont ? subSelectedStyle.textStyle : style.textStyle;
			}
			else {
				view.backgroundStyle = style.background;
				view.textStyle = style.textStyle;
			}
		}

		public int GetNextId() =>
				++lastNodeId;

		public Node FindNodeByIdRecursive(int id) =>
				root.FindNodeByIdRecursive(id, root);


		public Node[] FindNodesByStringContainsRecursive(string searchName, bool ignoreCase = true) {
			var foundedItems = new List<Node>();
			root.FindNodesByStringContainsRecursive(searchName, foundedItems, ignoreCase);
			return foundedItems.ToArray();
		}

		public Node[] FindNodesByNameRecursive(string searchName) {
			var foundedItems = new List<Node>();
			root.FindNodesByFullNameRecursive(searchName, foundedItems);
			return foundedItems.ToArray();
		}

		public void FocusOnSelected() {
			if (!hasSelected)
				return;

			FocusOn(_selectedNode);
		}

		public void FocusOn(Node node) {
			if (node == null || node.tree == null)
				return;

			if (node.tree != this)
				return;

			var allNodeParents = node.GetAllParentsRecursive();

			//Expand all parents
			foreach (var parent in allNodeParents)
				parent.ExpandWithoutNotify();

			Rebuild();

			_focusedNodeIndexInExpandedNodesList = GetIndexInExpandedNodesList(node);

			// setting focus continues in method AfterReload, after tree rebuild finished.
			//flag that indicates the need to focus
			_isAwaitReloadForFocusOn = true;
		}

		public Node FindFirstNodeByDataRecursive(object searchedData) =>
				rootNode.FindNodeByDataRecursive(searchedData);

		private int GetIndexInExpandedNodesList(Node node) {
			for (var i = 0; i < expandedNodes.Count; i++) {
				if (node == expandedNodes[i])
					return i;
			}
			return -1;
		}

		private void SelectAndNotify(Node node) {
			_selectedNode = node;
			_selectedNode.SetSelectedWithoutNotify(true);

			if (highlightSubSelected)
				node.ChangeIsSubSelectedStateForAllChildren(true);

			onNodeSelected?.Invoke(node);
		}

		private void DeselectAndNotify(Node node) {
			_selectedNode = null;
			node.SetSelectedWithoutNotify(false);

			if (highlightSubSelected)
				node.ChangeIsSubSelectedStateForAllChildren(false);

			onNodeDeselected?.Invoke(node);
		}

		private void OnNodeClicked(Node node) {
			SelectedNode(node);
			onNodeClick?.Invoke(node);
		}
		public void SelectedNode(Node node) {
			if (expandOnSelect)
				OnNodeClickedExpandOnSelectMode(node);
			else
				OnNodeClickedRegular(node);
		}
		private void OnNodeExpandClicked(Node node) {
			if (expandOnSelect)
				OnNodeClickedExpandOnSelectMode(node);
			else
				OnNodeExpandClickedRegular(node);
		}

		private void OnNodeExpandClickedRegular(Node node) {
			if (!node.hasChildren)
				return;

			node.SetExpandedStateWithoutNotify(!node.isExpanded);
			Rebuild();

			onNodeExpandStateChanged?.Invoke(node);
		}
		private void OnNodeClickedRegular(Node node) {
			if (_selectedNode == null) {
				SelectAndNotify(node);
			}
			else if (_selectedNode == node) {
				DeselectAndNotify(node);
			}
			else {
				DeselectAndNotify(_selectedNode);
				SelectAndNotify(node);
			}

			base.Repaint();
			onSelectionChanged?.Invoke(node);
		}
		public void OnNodeClickedExpandOnSelectMode(Node node) {
			if (_selectedNode == null) {
				SelectAndNotify(node);
			}
			/*else if (_selectedNode == node && !expandOnSelect) {
				DeselectAndNotify(node);
			}*/
			else if (_selectedNode != node) {
				DeselectAndNotify(_selectedNode);
				SelectAndNotify(node);

				onSelectionChanged?.Invoke(node);
			}

			if (node.hasChildren) {
				node.SetExpandedStateWithoutNotify(!node.isExpanded);
				Rebuild();
				onNodeExpandStateChanged?.Invoke(node);
			}
			else {
				base.Repaint();
			}
		}

		private void NodeCheckedClicked(Node node) {
			var newCheckedState = !node.isChecked;
			node.SetCheckedWithoutNotify(newCheckedState);
			if (isRecursiveChecked)
				node.ChangeIsCheckedStateForAllChildren(newCheckedState);
			UpdateNodeCheckedState(node);
		}

		private void OnNodeDoubleClick(Node node) =>
				onNodeDblClick?.Invoke(node);

		protected override RecycleItem CreateItem() {
			var item = Instantiate(Resources.Load<NodeView>(ITEM_RESOURCE_NAME), content, false);
			item.treePrefs = GetTreePrefs();
			return item;
		}

		protected override void OnPoolIncrease(RecycleItem item) =>
				((NodeView)item).NodeWidthReadyEvent += UpdateContentWidth;

		protected override void BeforePoolDecrease(RecycleItem item) =>
				((NodeView)item).NodeWidthReadyEvent -= UpdateContentWidth;

		protected override void BeforeReload() =>
				_maxItemWidth = 0;

		protected override void AfterReload() {
			//Focus On Continue
			if (_isAwaitReloadForFocusOn) {
				base.SetTopmostIndexForFocusedNode(_focusedNodeIndexInExpandedNodesList);
				base.UpdateVerticalScrollbar(default);
				_isAwaitReloadForFocusOn = false;
			}

			if (!fullRowNodes)
				return;

			var viewportWidth = viewport.rect.width;
			_maxItemWidth = viewportWidth;
			SetContentWidth(_maxItemWidth /*+ contentPadding.left + contentPadding.right*/);
		}

		private void UpdateContentWidth(float itemWidth) {
			if (itemWidth < _maxItemWidth)
				return;
			_maxItemWidth = itemWidth;
			SetContentWidth(_maxItemWidth + contentPadding.left + contentPadding.right);
		}

		private void SetContentWidth(float width) =>
				content.sizeDelta = new Vector2(width, content.sizeDelta.y);

		protected override void Awake() {
			if (root != null)
				return;
			lastNodeId = -1;
			root = new Node(this);
			root.isExpanded = true;
		}

		protected override void OnEnable() {
			base.OnEnable();
			recycleDataSource = this;
			Rebuild();
		}

		private TreePrefs GetTreePrefs() => new() {
				fullRectSelect = fullRowNodes,
				childIndent = childIndent,
				toggleWidth = toggleWidth,
				toggleIconSize = toggleIconSize,
				iconEnabled = imageEnabled,
				iconWidth = imageWidth,
				iconSize = imageIconSize,
				checkboxEnabled = checkboxEnabled,
				checkedWidth = checkboxWidth,
				checkedIconSize = checkboxIconSize,
				leftPadding = leftPadding,
				rightPadding = rightPadding,
				spacing = contentSpacing,

				hideExpandToggleWhenHasNoChildren = hideExpandToggleWhenHasNoChildren
		};
	}
}
