#if UNITY_EDITOR
using UnityEditor;

namespace SerializableCollections.GUI
{
	public sealed class DrawerState
	{
		public SerializedProperty Property { get; set; }
		public float Height { get; set; }

		public DrawerState(SerializedProperty property)
		{
			Property = property;
		}
	}
}
#endif
