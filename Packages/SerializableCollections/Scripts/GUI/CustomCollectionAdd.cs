namespace SerializableCollections.GUI
{
	public interface CustomCollectionAdd<T>
	{
		T Value { get; }
		void AddTemp();
		void ClearTemp();
	}
}