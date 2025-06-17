using System;
using UnityEngine;
namespace UIRecycleTreeNamespace {
	public class Example2 : MonoBehaviour {
		[SerializeField] private UIRecycleTree treeView;
		[SerializeField] private GameObject targetGameObject;
		[SerializeField] private Material selectedMaterial;
		[SerializeField] private Material regularMaterial;
		private Camera _cam;
		private void Awake() {
			_cam = Camera.main;

			FillTreeRecursive(treeView.rootNode, targetGameObject.transform);
			//Expand all nodes in tree
			treeView.ExpandAll();
		}

		private void Start() {
			treeView.ExpandAll();
		}

		public void OnEnable() =>
				//Subscribe on node checked state change event
				treeView.onNodeCheckedChanged.AddListener(OnNodeCheckedStateChanged);

		//Fill treeView recursive
		private void FillTreeRecursive(Node node, Transform targetTransform) {
			foreach (Transform childTransform in targetTransform) {
				//Add node to parent node
				var newNode = node.nodes.AddFluent(childTransform.name);
				// field "data" has object type, assign this field Transform reference
				newNode.data = childTransform;

				if (targetTransform.childCount > 0)
					FillTreeRecursive(newNode, childTransform);
			}
		}

		private void Update() {
			//Get Mouse click
			if (!Input.GetMouseButtonDown(0)) 
				return;
			//Create ray from screen touch point 
			var ray = _cam.ScreenPointToRay(Input.mousePosition);

			//Try Raycast along ray
			if (!Physics.Raycast(ray, out var hit)) return;
			//If hit, try find tree node, to which the reference to the hitted transform has been attached
			var node = treeView.FindFirstNodeByDataRecursive(hit.transform);
			if (node != null)
					//changing node.isChecked call event OnNodeCheckedStateChanged
				node.isChecked = !node.isChecked;
		}

		private void OnNodeCheckedStateChanged(Node node) {
			//Try cast node data field to <Transform>
			//You can use (Transform)node.data;
			if (!node.TryCastData<Transform>(out var assignedTransform)) return;
			
			//Change material
			var rend = assignedTransform.GetComponent<Renderer>();
			rend.material = node.isChecked ? selectedMaterial : regularMaterial;
		}

	}
}