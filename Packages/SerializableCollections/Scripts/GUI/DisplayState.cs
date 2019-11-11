#if UNITY_EDITOR
using UnityEditor;

namespace SerializableCollections.GUI
{
	public class DisplayState
	{
		public SerializedProperty Collection { get; set; }
		public ReorderableCollection List { get; set; }
		public bool ElementsDisabled { get; set; }
		private bool addingElement;
		public bool AddingElement { get { return addingElement; } set { addingElement = value; List.List.draggable = !value; } }
		public float LastAddingHeight { get; set; }
		public float LastHeaderHeight { get; set; }
		public float LastFooterHeight { get; set; }
		public bool EditingSettings { get; set; }
		public int CurrentPage { get; set; }
		public int CurrentPageSet { get; set; }
		public int PageSetSize { get; set; }
	}
}
#endif
