using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIRecycleTreeNamespace {
	[Serializable]
	public class NodeCollection : IList<Node> {
		public int Count => _childNodes.Count;

		[SerializeReference] private List<Node> _childNodes;
		[SerializeReference] private Node _owner;

		private bool isOwnerHasTree => _owner.tree != null;
		public NodeCollection(Node ownerNode) {
			_owner = ownerNode;
			_childNodes = new();
		}

		public void AddRange(Node[] nodeArray) {
			foreach (var node in nodeArray)
				AddInternal(node);
			if (!isOwnerHasTree) return;
			RebuildTree();
		}
		
		public Node AddFluent(string name, int styleIndex) {
			var node = new Node(name, styleIndex);
			return AddFluent(node);
		}

		public Node AddFluent(string name) {
			var node = new Node(name);
			return AddFluent(node);
		}

		public Node AddFluent(Node node) {
			Add(node);
			return node;
		}

		public void Add(Node node) {
			if (node == null) return;
			AddInternal(node);
			if (isOwnerHasTree && node.CheckAllParentExpanded())
				RebuildTree();
		}
		
		public void Clear() {
			_childNodes.Clear();
		
			if (isOwnerHasTree)
				RebuildTree();
		}

		private void AddInternal(Node node) {
			_childNodes.Add(node);
			node.parentNode = _owner;

			if (!isOwnerHasTree) return;
			AssignTreeToNodeAndAllChildren(node);
		}
		
		private void AssignTreeToNodeAndAllChildren(Node node) {
			var allChildNodes = node.GetAllChildrenRecursive();
			foreach (var childNodes in allChildNodes)
				childNodes.tree = _owner.tree;
		}

		

		public bool Remove(Node node) {
			if (node == null) return false;

			bool treeNotifyNeeded = isOwnerHasTree && node.CheckAllParentExpanded();
			
			var removed = _childNodes.Remove(node);
			if (!removed) return false;

			if (treeNotifyNeeded)
				RebuildTree();
			return true;
		}

		public void RemoveAt(int index) {
			bool treeNotifyNeeded = isOwnerHasTree && _childNodes[index].CheckAllParentExpanded();
			_childNodes.RemoveAt(index);

			if (treeNotifyNeeded)
				RebuildTree();
		}

		private void RebuildTree() =>
				_owner.tree.Rebuild();

		// IList standard properties implement

		public int IndexOf(Node node) =>
				_childNodes.IndexOf(node);

		public void Insert(int index, Node node) =>
				_childNodes.Insert(index, node);

		public void CopyTo(Node[] array, int arrayIndex) =>
				_childNodes.CopyTo(array, arrayIndex);

		public IEnumerator<Node> GetEnumerator() =>
				_childNodes.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() =>
				GetEnumerator();

		public Node this[int index] {
			get => _childNodes[index];
			set => _childNodes[index] = value;
		}

		public bool IsReadOnly => false;

		public bool Contains(Node item) =>
				_childNodes.Contains(item);
	}
}