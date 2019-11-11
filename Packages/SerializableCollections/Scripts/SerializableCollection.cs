using System;

namespace SerializableCollections
{
	public abstract class SerializableCollection
	{
#if UNITY_EDITOR
		public SerializableCollection()
		{
			RegisterForDrawer();
		}

		protected virtual void RegisterForDrawer()
		{
			Type currentType = GetType();
			while (currentType != null)
			{
				foreach (var attribute in currentType.CustomAttributes)
				{
					if (attribute.AttributeType == typeof(GUI.UsePropertyDrawer))
					{
						Type drawerType = attribute.ConstructorArguments[0].Value as Type;
						GUI.CollectionDrawer.RegisterForDrawer(drawerType, GetType());
					}
				}

				currentType = currentType.BaseType;
			}
	}
#endif
	}
}