using TMPro;
using UnityEngine;

namespace UIRecycleTreeNamespace {
	[CreateAssetMenu(menuName = "UIRecycleTree/NodeStyle", fileName = "NodeStyle", order = 0)]
	public class NodeStyle : ScriptableObject {
		[SerializeField] private ExpandIcons _toggleIcons;
		[SerializeField] private ExpandIcons _imageIcons;
		[SerializeField] private CheckboxIcons _checkboxIcons;

		[SerializeField] private Background _background;
		[SerializeField] private NodeTextStyle _nodeTextStyle;
		[SerializeField] private StateStyle _selectedState;
		[SerializeField] private StateStyle _subSelectedState;
		[SerializeField] private float _fadedAlpha = 0.3f;

		public NodeTextStyle textStyle => _nodeTextStyle;
		public Background background => _background;
		public StateStyle selectedState => _selectedState;
		public StateStyle subSelectedState => _subSelectedState;
		public ExpandIcons toggleIcons => _toggleIcons;
		public ExpandIcons imageIcons => _imageIcons;
		public CheckboxIcons checkboxIcons => _checkboxIcons;
		public float fadeAlpha => _fadedAlpha;
	}
}