using UnityEngine;

namespace UIRecycleTreeNamespace {
	public class Example4 : MonoBehaviour {
		[SerializeField] private UIRecycleTree treeView;
		private int _example3StyleIndex = 3;
		private void Start() {
			CreateNodes();
		}
		private void CreateNodes() {
			//You can create nodes using constructor 
			var animations = new Node("Animations", _example3StyleIndex);
			//to add a node to an existing one, you need to call the method "nodes.AddFluent"
			//method a can take as a parameter the string "name" of a new node or a created node
			//method a returns the created node

			// Creating the "Enemies" and "Mobs" nodes as children of the animation node using the constructor
			var character = animations.nodes.AddFluent(new Node("Character", _example3StyleIndex));
			var mobs = new Node("Mobs", _example3StyleIndex);
			mobs = animations.nodes.AddFluent(mobs);

			// Creating the "Enemies" node as a child of the animation node by simply specifying the name in the parameter
			var enemy = animations.nodes.AddFluent("Enemies", _example3StyleIndex);


			//It is possible to add nodes in a chain,
			//Lets add "Wolf" node to "mobs" node and then add "Idle" node to "Wolf" node in a chain
			var idle = mobs.nodes.AddFluent("Wolf", _example3StyleIndex).nodes.AddFluent("idle", _example3StyleIndex);
			idle.isFaded = true;

			//It is possible to add an array of nodes to a node using nodes.AddRange method
			var charactersArray = new Node[] {
					new Node("Male", _example3StyleIndex),
					new Node("Female", _example3StyleIndex),
					new Node("Child", _example3StyleIndex)
			};

			//Add an charactersArray to node "character"
			character.nodes.AddRange(charactersArray);

			//add an animation node with everything contained to the tree
			treeView.nodes.Add(animations);
		}
	}
}
