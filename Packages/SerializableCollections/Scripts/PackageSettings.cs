#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace SerializableCollections
{
	[ExecuteInEditMode]
	[CreateAssetMenu(menuName = "SerializableCollections Settings")]
	public sealed class PackageSettings : ScriptableObject
	{
		private static PackageSettings settings;
		public static PackageSettings Settings
		{
			get
			{
				if (settings == null)
				{
					var allSettings = AssetDatabase.FindAssets("t: PackageSettings");
					if (allSettings.Length > 0)
					{
						settings = AssetDatabase.LoadAssetAtPath<PackageSettings>(AssetDatabase.GUIDToAssetPath(allSettings[0]));
					}
					else
					{
						var newSettings = CreateInstance<PackageSettings>();
						AssetDatabase.CreateAsset(newSettings, "Assets/Serializable Collections Settings.asset");
						settings = newSettings;
					}
				}

				return settings;
			}
		}

		[SerializeField]
		private ViewSettings defaultListSettings = new ViewSettings();
		public ViewSettings DefaultListSettings { get { return defaultListSettings; } }

		[SerializeField]
		private ListLookup listViewSettings = new ListLookup();
		public ListLookup ListViewSettings { get { return listViewSettings; } }

		public ViewSettings GetListSettings(SerializedProperty collection)
		{
			string collectionPath = GetCollectionPath(collection);
			if (collectionPath == null || !listViewSettings.ContainsKey(collectionPath))
				return null;

			return listViewSettings[collectionPath];
		}

		public ViewSettings AddNewSettings(SerializedProperty collection)
		{
			ViewSettings settings = new ViewSettings(DefaultListSettings);
			string path = GetCollectionPath(collection);
			if (path != null)
				listViewSettings.Add(GetCollectionPath(collection), settings);
			else
				return null;

			return settings;
		}

		private string GetCollectionPath(SerializedProperty collection)
		{
			try
			{
				return collection.serializedObject.targetObject.GetType() + "." + collection.propertyPath;
			}
			catch (Exception)
			{
				return null;
			}
		}

		[Serializable]
		public class ListLookup : Dictionary<string, ViewSettings> { }

		[Serializable]
		public class ViewSettings
		{
			[SerializeField]
			private Color color = Color.white;
			public Color Color { get { return color; } set { color = value; } }

			[SerializeField]
			private bool usePages = true;
			public bool UsePages { get { return usePages; } set { usePages = value; } }

			[SerializeField]
			private int itemsPerPage = 10;
			public int ItemsPerPage { get { return itemsPerPage; } set { itemsPerPage = value; } }

			[SerializeField]
			private string keyLabel = default;
			public string KeyLabel { get { return keyLabel; } set { keyLabel = value; } }

			[SerializeField]
			private string valueLabel = default;
			public string ValueLabel { get { return valueLabel; } set { valueLabel = value; } }

			public ViewSettings() { }
			public ViewSettings(ViewSettings source)
			{
				color = source.color;
				usePages = source.usePages;
				itemsPerPage = source.itemsPerPage;
			}
		}
	}
}
#endif
