
namespace UIRecycleTreeNamespace {
	public interface IRecycleDataSource {
		int expandedCount { get; }
		void MergeDataWithView(RecycleItem recycleItem, int indexInExpandedNodes);
	}
}
