using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
namespace UIRecycleTreeNamespace {
	public class Example : MonoBehaviour {
		[SerializeField] private UIRecycleTree treeView;
		[SerializeField] protected Button button, expand, collapse, delete, fadeButton, focusButton;
		[SerializeField] private TMP_Text pathTextField, nodesCountText;
		private void Start() {
			//Set fps max framerate for testing
			Application.targetFrameRate = 100;
			FillTree();
			//Subscribing on button events
			button.onClick.AddListener(FillTree);
			expand.onClick.AddListener(Expand);
			collapse.onClick.AddListener(Collapse);
			delete.onClick.AddListener(DeleteSelected);
			fadeButton.onClick.AddListener(FadeSelected);

			//
			focusButton.onClick.AddListener(FocusSelected);
			//

			//Subscribing on tree events
			treeView.onNodeSelected.AddListener(OnNodeSelect);
			treeView.onNodeDblClick.AddListener(DoubleClicked);
			treeView.onSelectionChanged.AddListener(SelectionChanged);
			treeView.onNodeExpandStateChanged.AddListener(OnExpand);
			treeView.onNodeCheckedChanged.AddListener(OnChecked);
		}
		
		//FocusOn Selected
		private void FocusSelected() =>
			treeView.FocusOnSelected();
		
		private void OnChecked(Node node) {
			//When node isChecked state change
			Debug.Log($"{node.name} isChecked =  {node.isChecked}");
		}

		private void SelectionChanged(Node node) {
			//When tree selection change
			Debug.Log($"{node.name} Selection Changed");
		}

		private void OnExpand(Node node) {
			//When node isExpanded state changed
			Debug.Log($"{node.name} isExpanded =  {node.isExpanded}");
		}

		private void FadeSelected() {
			// When fade button click
			if (treeView.hasSelected)
				treeView.selectedNode.isFaded = !treeView.selectedNode.isFaded;
		}

		private void DoubleClicked(Node node) {
			//On Node dlb clicked
			Debug.Log($"{node.name} double clicked");
		}

		private void OnNodeSelect(Node node) {
			// On Tree Node selected
			DrawPath(node);
			Debug.Log($"{node.name} is selected");
		}
		private void DrawPath(Node node) =>
				pathTextField.text = node.fullPath;

		private void DeleteSelected() {
			//Check if tree dont have selected node return
			if (!treeView.hasSelected) return;
			var selectedNode = treeView.selectedNode;
			var selectedName = selectedNode.name;
			// Remove Node
			selectedNode.RemoveYourself();

			Debug.Log($"{selectedName} node deleted");
			nodesCountText.text = treeView.nodesCount.ToString();
		}

		private void Collapse() =>
				treeView.CollapseAll();

		private void Expand() =>
				treeView.ExpandAll();

		//Fill tree randomly
		private void FillTree() {
			if (treeView.nodesCount != 0)
				treeView.Clear();
			// Generate random data Max child count 12 max depth 5 
			GenerateRandomTreeContent(treeView.rootNode, 12, 5);
			nodesCountText.text = treeView.nodesCount.ToString();
		}

		//Generate random tree data
		private void GenerateRandomTreeContent(Node node, int maxChildCount, int maxDepth) {
			if (maxDepth <= 0) return;
			//for each node we randomly determine the number of children
			var childCount = Random.Range(1, maxChildCount);

			for (int i = 0; i <= childCount; i++) {
				//create child
				var newNode = node.nodes.AddFluent(new Node());
				//set child name
				newNode.name = $"id{newNode.nodeId}[depth{node.depth + 1}]";
				//call this method recursive, but change maxDepth
				GenerateRandomTreeContent(newNode, maxChildCount, maxDepth - 1);
			}
		}
	}
}
