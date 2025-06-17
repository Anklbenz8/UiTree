using UnityEngine;

namespace UIRecycleTreeNamespace {
	public class AddNodesFromCodeExample : MonoBehaviour {
		[SerializeField] private UIRecycleTree treeView;
		private void Start() {
			CreateNodes();
		}

		private void CreateNodes() {
			//You can create nodes using constructor
			var animations = new Node("Animations");
			//to add a node to an existing one, you need to call the method "nodes.AddFluent"
			//method a can take as a parameter the string "name" of a new node or a created node
			//method a returns the created node

			// Creating the "Enemies" and "Mobs" nodes as children of the animation node using the constructor
			var character = animations.nodes.AddFluent(new Node("Character"));
			var mobs = new Node("Mobs");
			mobs = animations.nodes.AddFluent(mobs);

			// Creating the "Enemies" node as a child of the animation node by simply specifying the name in the parameter
			var enemy = animations.nodes.AddFluent("Enemies");


			//It is possible to add nodes in a chain,
			//Lets add "Wolf" node to "mobs" node and then add "Idle" node to "Wolf" node in a chain
			var idle = mobs.nodes.AddFluent("Wolf").nodes.AddFluent("idle");
			idle.isFaded = true;

			//It is possible to add an array of nodes to a node using nodes.AddRange method
			var charactersArray = new Node[] {
					new Node("Male"),
					new Node("Female"),
					new Node("Child")
			};

			//Add an charactersArray to node "character"
			character.nodes.AddRange(charactersArray);

			//add an animation node with everything contained to the tree
			treeView.nodes.Add(animations);
		}
	}
}
