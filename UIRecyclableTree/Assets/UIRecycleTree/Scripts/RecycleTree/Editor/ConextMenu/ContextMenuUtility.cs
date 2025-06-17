using UnityEditor;
namespace UIRecycleTreeNamespace {
	public static class ContextMenuUtility {
		private const string ELEMENT_NAME_IN_RESOURCES = "UIRecycleTree"; 
		
		[MenuItem("GameObject/UIComponents/RecycleTree")]
		public static void CreateSwitcher(MenuCommand menuCommand) {
			CreateUtility.CreateUIElement(ELEMENT_NAME_IN_RESOURCES);
		}
	}
}