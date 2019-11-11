#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SerializableCollections.GUI
{
	[DisplayType(typeof(LinkedListDisplay))]
	public class LinkedListDrawer : CollectionDrawer
	{
	}

	public class LinkedListDisplay : CollectionDisplay
	{
		protected override Rect DrawCollection(Rect position, SerializedProperty property, SerializedProperty collection, GUIContent label)
		{
			ReorderableCollection.ListStates[GetFullPath(collection)].List.DrawCustomAdd = true;
			return base.DrawCollection(position, property, collection, label);
		}
	}
}
#endif