using System.Collections.Generic;

namespace SerializableCollections
{
	public interface SortedCollection<KeyType>
	{
		IComparer<KeyType> Comparer { get; }
	}
}
