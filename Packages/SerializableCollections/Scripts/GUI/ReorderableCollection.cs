using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace SerializableCollections.GUI
{
	public class ReorderableCollection
	{
#if UNITY_EDITOR
		public delegate float DrawAreaHandler(Rect position, SerializedProperty property, SerializedProperty collection);
		public delegate void DrawElementHandler(Rect position, int index, SerializedProperty property, SerializedProperty collection);
		public delegate float ElementHeightHandler(int index, SerializedProperty property, SerializedProperty collection);
		public delegate void ReorderHandler(SerializedProperty property, SerializedProperty collection, int oldIndex, int newIndex);

		public static Dictionary<string, DisplayState> ListStates { get; private set; } = new Dictionary<string, DisplayState>();
		public static readonly Texture2D alternatingBGColor = new Texture2D(1, 1);
		public static readonly Texture2D selectedColor = new Texture2D(1, 1);
		private static readonly GUIStyle linkStyle = new GUIStyle(EditorStyles.label);
		private static readonly GUIStyle headerBackgroundStyle = new GUIStyle("RL Header");
		private static readonly GUIStyle footerBackgroundStyle = new GUIStyle("RL Footer");
		private static readonly GUIStyle centeredLabelStyle = new GUIStyle(EditorStyles.boldLabel);
		private static readonly GUIStyle addRemButtonStyle = new GUIStyle("Button");
		private static readonly GUIStyle activeSettingsBackground = new GUIStyle();
		private static readonly GUIStyle boxBackground = "RL Background";
		private static readonly Texture2D dividerTexture = new Texture2D(1, 1);

		static ReorderableCollection()
		{
			Selection.selectionChanged += () =>
			{
				ListStates.Clear();
			};

			EditorApplication.hierarchyChanged += () =>
			{
				ListStates.Clear();
			};

			centeredLabelStyle.alignment = TextAnchor.UpperCenter;
			dividerTexture.SetPixel(0, 0, (EditorGUIUtility.isProSkin) ? Color.white : Color.black);
			activeSettingsBackground.normal.background = dividerTexture;
			alternatingBGColor.SetPixel(0, 0, (EditorGUIUtility.isProSkin) ? new Color(.25f, .25f, .25f) : new Color(.75f, .75f, .75f));
			selectedColor.SetPixel(0, 0, UnityEngine.GUI.skin.settings.selectionColor);
			linkStyle.normal.textColor = Color.blue;
			linkStyle.hover.textColor = Color.cyan;
			addRemButtonStyle.alignment = TextAnchor.MiddleCenter;
			addRemButtonStyle.fontSize = 14;
			addRemButtonStyle.fontStyle = FontStyle.Bold;
		}

		public static int GetItemsPerPage(SerializedProperty collection)
		{
			PackageSettings.ViewSettings settings = PackageSettings.Settings.GetListSettings(collection);
			PackageSettings.ViewSettings defaultSettings = PackageSettings.Settings.DefaultListSettings;
			int pageSize;
			if (settings == null)
				pageSize = (defaultSettings.UsePages) ? defaultSettings.ItemsPerPage : collection.arraySize;
			else
				pageSize = (settings.UsePages) ? settings.ItemsPerPage : collection.arraySize;

			return pageSize;
		}

		public static Vector2Int GetPageDisplayRange(SerializedProperty collection)
		{
			DisplayState state = ListStates[CollectionDisplay.GetFullPath(collection)];
			int pageSize = GetItemsPerPage(state.Collection);
			int pageStartIndex = (state.CurrentPageSet * state.PageSetSize) + (state.CurrentPage * pageSize);
			int pageEndIndex = pageStartIndex + pageSize;
			return new Vector2Int(pageStartIndex, pageEndIndex);
		}

		private float DrawSettingsButton(Rect position, SerializedProperty property, SerializedProperty collection)
		{
			var state = ListStates[CollectionDisplay.GetFullPath(collection)];
			Rect settingsPosition = position;
			settingsPosition.xMin = (settingsPosition.xMax - 20) + 7;
			settingsPosition.xMax += 7;
			settingsPosition.y += 4;
			settingsPosition.height = 20;

			GUIContent iconContent = EditorGUIUtility.IconContent("_Popup");
			GUIStyle buttonStyle = centeredLabelStyle;
			Color guiColor = UnityEngine.GUI.color;
			if (state.EditingSettings)
			{
				UnityEngine.GUI.color = Color.Lerp(guiColor, Color.yellow, .3f);
				buttonStyle = activeSettingsBackground;
				buttonStyle.fixedHeight = settingsPosition.height - 4;
				buttonStyle.fixedWidth = settingsPosition.width - 4;
			}

			if (UnityEngine.GUI.Button(settingsPosition, iconContent.image, buttonStyle))
			{
				state.EditingSettings = !state.EditingSettings;
			}

			UnityEngine.GUI.color = guiColor;
			return 0;
		}

		private static float DrawSettings(Rect position, SerializedProperty property, SerializedProperty collection)
		{
			var listState = ListStates[CollectionDisplay.GetFullPath(collection)];

			if (!listState.EditingSettings)
			{
				return 0;
			}

			Rect startPosition = position;

			position.xMin -= 6;
			position.xMax += 6;

			PackageSettings.ViewSettings settings = PackageSettings.Settings.GetListSettings(collection);
			PackageSettings.ViewSettings defaultSettings = PackageSettings.Settings.DefaultListSettings;
			int pageSize = (settings == null) ? defaultSettings.ItemsPerPage : settings.ItemsPerPage;
			position.xMin += 7;
			position.xMax -= 7;
			position.y += 4;

			position.height = EditorStyles.boldLabel.CalcHeight(new GUIContent("List Settings"), position.width);
			UnityEngine.GUI.Label(position, new GUIContent("List Settings"), EditorStyles.boldLabel);
			position.y += position.height;

			bool usePages = (settings == null) ? defaultSettings.UsePages : settings.UsePages;
			bool newUsePages = EditorGUI.Toggle(position, "Use Page System", usePages);
			position.y += position.height;
			if (usePages != newUsePages)
			{
				if (settings == null)
					settings = PackageSettings.Settings.AddNewSettings(collection);

				settings.UsePages = newUsePages;
				listState.CurrentPage = 0;
				EditorUtility.SetDirty(PackageSettings.Settings);
			}

			float fieldHeight = EditorStyles.textField.fixedHeight;
			position.height = EditorGUIUtility.singleLineHeight;
			EditorStyles.textField.fixedHeight = position.height;
			if (newUsePages)
			{
				int newPageSize = EditorGUI.DelayedIntField(position, "Items Per Page", pageSize);
				if (newPageSize < 1)
					newPageSize = Mathf.Clamp(newPageSize, 1, 100);

				if (newPageSize != pageSize)
				{
					if (settings == null)
						settings = PackageSettings.Settings.AddNewSettings(collection);

					settings.ItemsPerPage = newPageSize;
					listState.CurrentPage = 0;
					EditorUtility.SetDirty(PackageSettings.Settings);
				}
				position.y += position.height + 4;
			}

			Color settingsColor = (settings == null) ? defaultSettings.Color : settings.Color;
			Color newColor = EditorGUI.ColorField(position, "List Color", settingsColor);
			if (newColor != settingsColor)
			{
				if (settings == null)
					settings = PackageSettings.Settings.AddNewSettings(collection);

				settings.Color = newColor;
				EditorUtility.SetDirty(PackageSettings.Settings);
			}
			position.y += position.height + 4;

			EditorStyles.textField.fixedHeight = fieldHeight;
			position.height = 2;
			UnityEngine.GUI.DrawTexture(position, dividerTexture);
			position.y += position.height + 2;
			return position.y - startPosition.y;
		}

		private static float DrawPageSelection(Rect position, SerializedProperty property, SerializedProperty collection)
		{
			Rect startPosition = position;
			var state = ListStates[CollectionDisplay.GetFullPath(collection)];
			int currentPage = state.CurrentPage;
			int pageSize = GetItemsPerPage(collection);
			int pages = Mathf.CeilToInt(collection.arraySize / (float)pageSize);

			Rect pagePosition = position;
			pagePosition.height = EditorGUIUtility.singleLineHeight + 5;
			pagePosition.xMax = pagePosition.xMin + 20;
			float availableWidth = position.width - 100;
			int maxPossiblePageButtons = Mathf.RoundToInt(Mathf.Abs(Mathf.Ceil(availableWidth / pagePosition.width))) - 2;
			int pageSets = Mathf.CeilToInt(pages / (float)maxPossiblePageButtons);
			int startPage = state.CurrentPage;
			int startPageSet = state.CurrentPageSet;

			if ((state.CurrentPageSet * maxPossiblePageButtons) >= pages)
				state.CurrentPageSet = 0;

			int pageMin = (state.CurrentPageSet * maxPossiblePageButtons);
			int pageMax = pageMin + maxPossiblePageButtons;
			if (pageMax >= pages)
				pageMax = pages;

			if (Event.current.type == EventType.Repaint)
			{
				state.PageSetSize = maxPossiblePageButtons;

				if (state.CurrentPage >= (pageMax - pageMin))
					state.CurrentPage = 0;
				if (state.CurrentPage < 0)
					state.CurrentPage = (pageMax - pageMin) - 1;
			}

			if (pages > 1)
			{
				if (state.CurrentPageSet > 0)
				{
					EditorGUIUtility.AddCursorRect(pagePosition, MouseCursor.Link);
					if (UnityEngine.GUI.Button(pagePosition, EditorGUIUtility.TrIconContent("Animation.FirstKey"), linkStyle))
					{
						EditorGUIUtility.AddCursorRect(pagePosition, MouseCursor.Link);
						state.CurrentPageSet--;
					}
				}

				pagePosition.x += pagePosition.width;

				if (currentPage + state.CurrentPageSet > 0)
				{
					EditorGUIUtility.AddCursorRect(pagePosition, MouseCursor.Link);
					if (UnityEngine.GUI.Button(pagePosition, EditorGUIUtility.TrIconContent("Animation.PrevKey", "Previous page"), linkStyle))
					{
						EditorGUIUtility.AddCursorRect(pagePosition, MouseCursor.Link);
						state.CurrentPage--;

						if (state.CurrentPage < 0)
						{
							state.CurrentPageSet--;
							state.CurrentPage = maxPossiblePageButtons - 1;
						}
					}
				}

				pagePosition.x += pagePosition.width;

				for (var i = pageMin; i < pageMax; i++)
				{
					GUIContent label = new GUIContent((i + 1).ToString());
					if (i != state.CurrentPage + pageMin)
					{
						EditorGUIUtility.AddCursorRect(pagePosition, MouseCursor.Link);
						if (UnityEngine.GUI.Button(pagePosition, label, linkStyle))
						{
							state.CurrentPage = i - pageMin;
						}
					}
					else
					{
						UnityEngine.GUI.Label(pagePosition, label, EditorStyles.boldLabel);
						Rect underlinePos = pagePosition;
						EditorStyles.boldLabel.CalcMinMaxWidth(label, out _, out float maxWidth);
						underlinePos.width = maxWidth;
						underlinePos.yMax -= 3;
						underlinePos.yMin = underlinePos.yMax - 2;
						EditorGUI.DrawRect(underlinePos, EditorStyles.boldLabel.normal.textColor);
					}

					pagePosition.x += pagePosition.width;
				}

				if (currentPage + pageMin < (pages - 1))
				{
					EditorGUIUtility.AddCursorRect(pagePosition, MouseCursor.Link);
					if (UnityEngine.GUI.Button(pagePosition, EditorGUIUtility.TrIconContent("Animation.NextKey", "Next Page."), linkStyle))
					{
						state.CurrentPage++;
						if (state.CurrentPage + (state.CurrentPageSet * maxPossiblePageButtons) >= pageMax)
						{
							state.CurrentPageSet++;
							state.CurrentPage = 0;
						}
					}

					pagePosition.x += pagePosition.width;
				}

				if (state.CurrentPageSet < pageSets - 1)
				{
					EditorGUIUtility.AddCursorRect(pagePosition, MouseCursor.Link);
					if (UnityEngine.GUI.Button(pagePosition, EditorGUIUtility.TrIconContent("Animation.LastKey"), linkStyle))
					{
						EditorGUIUtility.AddCursorRect(pagePosition, MouseCursor.Link);
						state.CurrentPageSet++;
					}
				}
			}

			if (state.CurrentPage != startPage || state.CurrentPageSet != startPageSet)
			{
				EditorGUIUtility.editingTextField = false;
				state.List.List.index = -1;
			}

			position.y += pagePosition.height;
			return position.y - startPosition.y;
		}

		private static float DrawHeaderBackground(Rect position, SerializedProperty property, SerializedProperty collection)
		{
			var listState = ListStates[CollectionDisplay.GetFullPath(collection)];
			Rect bgPosition = position;
			bgPosition.xMin -= 6;
			bgPosition.xMax += 6;
			DrawAreaBackground(bgPosition, headerBackgroundStyle, listState.LastHeaderHeight + 2);
			return 0;
		}

		private static float DrawFooterBackground(Rect position, SerializedProperty property, SerializedProperty collection)
		{
			var listState = ListStates[CollectionDisplay.GetFullPath(collection)];
			position.yMin -= 2;
			DrawAreaBackground(position, footerBackgroundStyle, listState.LastFooterHeight);
			return 0;
		}

		private static void DrawAreaBackground(Rect position, GUIStyle style, float height)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Rect backgroundPos = position;
				backgroundPos.height = height;
				backgroundPos.yMax = backgroundPos.yMin + height;
				style.fixedHeight = backgroundPos.height;
				style.fixedWidth = backgroundPos.width;
				style.Draw(backgroundPos, GUIContent.none, 2);
			}
		}

		private static float DrawAddRemoveButtons(Rect position, SerializedProperty property, SerializedProperty collection)
		{
			var state = ListStates[CollectionDisplay.GetFullPath(collection)];

			Rect addButtonPosition = position;
			addButtonPosition.height = 20;
			addButtonPosition.xMax -= 3;
			addButtonPosition.xMin = addButtonPosition.xMax - 20;

			if (UnityEngine.GUI.Button(addButtonPosition, "+", addRemButtonStyle))
			{
				state.List.List.onAddCallback.Invoke(state.List.List);
				state.ElementsDisabled = false;
			}

			Rect remButtonPosition = addButtonPosition;
			remButtonPosition.x -= 20;
			if (collection.arraySize > 0 && state.List.List.index > -1)
				if (UnityEngine.GUI.Button(remButtonPosition, "-", addRemButtonStyle))
					state.List.List.onRemoveCallback.Invoke(state.List.List);

			return 0;
		}

		public event ReorderHandler Reorder;
		public event DrawElementHandler DrawElement;
		public DrawAreaHandler DrawAddElement { get; set; }
		public ElementHeightHandler GetElementHeight;
		public ReorderableList List;
		public bool DrawCustomAdd { get; set; }

		public List<DrawAreaHandler> DrawHeaderAreaHandlers = new List<DrawAreaHandler>();
		public List<DrawAreaHandler> DrawFooterAreaHandlers = new List<DrawAreaHandler>();

		public ReorderableCollection(SerializedProperty property, SerializedProperty collection)
		{
			bool allowDrag = true;
			object pathReference = SerializationReflection.GetPathReference(property);
			if (pathReference != null)
			{
				Type sortedType = SerializationReflection.GetPathReference(property).GetType();

				while (sortedType != null)
				{
					if (sortedType.GetInterface("SortedCollection`1") != null)
					{
						allowDrag = false;
						break;
					}
					sortedType = sortedType.BaseType;
				}
			}

			string collectionPath = CollectionDisplay.GetFullPath(collection);

			DisplayState listState;
			if (ListStates.ContainsKey(collectionPath))
				ListStates.Remove(collectionPath);

			listState = new DisplayState();
			ListStates.Add(collectionPath, listState);
			listState.Collection = collection;
			listState.List = this;

			DrawerState drawerState = CollectionDisplay.GetDrawerState(property);
			var newList = new ReorderableList(collection.serializedObject, collection)
			{
				draggable = allowDrag,
			};
			newList.showDefaultBackground = false;

			newList.onReorderCallbackWithDetails = (ReorderableList list, int oldIndex, int newIndex) =>
			{
				Reorder?.Invoke(drawerState.Property, listState.Collection, oldIndex, newIndex);
			};

			newList.drawHeaderCallback = (Rect position) =>
			{
				SerializedProperty hasLostValue = drawerState.Property.FindPropertyRelative("hasLostValue");
				if (hasLostValue != null && hasLostValue.boolValue == true)
				{
					EditorUtility.DisplayDialog("Collection Error", "You've attempted to change an element in a way that prevents it from being added to the collection. The element will be moved to the Add Element area of the list so you can change the element and add it back in.", "Okay");
					ListStates[CollectionDisplay.GetFullPath(listState.Collection)].AddingElement = true;
					hasLostValue.boolValue = false;
				}

				Rect startPosition = position;

				DrawHeaderBackground(position, drawerState.Property, listState.Collection);
				DrawSettingsButton(position, drawerState.Property, listState.Collection);
				position.y += DrawSettings(position, drawerState.Property, listState.Collection);

				for (var i = 0; i < DrawHeaderAreaHandlers.Count; i++)
				{
					position.y += DrawHeaderAreaHandlers[i].Invoke(position, drawerState.Property, listState.Collection);
				}

				listState.LastHeaderHeight = position.y - startPosition.y;
				newList.headerHeight = listState.LastHeaderHeight;
			};

			newList.drawElementCallback += (Rect position, int index, bool isActive, bool isFocused) =>
			{
				position.y += 2;

				var pageRange = GetPageDisplayRange(listState.Collection);
				if (index >= pageRange.x && index < pageRange.y)
				{
					EditorGUI.BeginDisabledGroup(listState.AddingElement);
					DrawElement?.Invoke(position, index, drawerState.Property, listState.Collection);
					EditorGUI.EndDisabledGroup();
				}
			};

			newList.drawElementBackgroundCallback = (Rect position, int index, bool isActive, bool isFocused) =>
			{
				if (Mathf.Approximately(position.height, 0))
					return;

				Rect backgroundPos = position;
				backgroundPos.yMin -= 2;
				backgroundPos.yMax += 2;
				boxBackground.fixedHeight = backgroundPos.height;
				boxBackground.fixedWidth = backgroundPos.width;
				Color guiColor = UnityEngine.GUI.color;

				if (isActive)
					UnityEngine.GUI.color = UnityEngine.GUI.skin.settings.selectionColor;

				if (index % 2 == 0)
				{
					UnityEngine.GUI.color = Color.Lerp(UnityEngine.GUI.color, Color.black, .1f);
				}

				if (Event.current.type == EventType.Repaint)
					boxBackground.Draw(position, false, isActive, true, false);

				UnityEngine.GUI.color = guiColor;

				position.xMin += 2;
				position.xMax -= 3;
			};

			newList.elementHeightCallback += (int index) =>
			{
				int pageSize = GetItemsPerPage(listState.Collection);
				int pageStartIndex = (listState.CurrentPageSet * listState.PageSetSize * pageSize) + (listState.CurrentPage * pageSize);
				int pageEndIndex = pageStartIndex + pageSize;

				if (index >= pageStartIndex && index < pageEndIndex && (GetElementHeight != null))
				{
					return GetElementHeight(index, drawerState.Property, listState.Collection) + 2;
				}

				return 0;
			};

			newList.drawFooterCallback = (Rect position) =>
			{
				DrawFooterBackground(position, drawerState.Property, listState.Collection);
				DrawAddRemoveButtons(position, drawerState.Property, listState.Collection);

				Rect startPosition = position;
				for (var i = 0; i < DrawFooterAreaHandlers.Count; i++)
				{
					position.y += DrawFooterAreaHandlers[i].Invoke(position, drawerState.Property, listState.Collection);
				}

				position.y += DrawPageSelection(position, drawerState.Property, listState.Collection);
				if (listState.List.DrawCustomAdd)
					position.y += DrawAddArea(position, property, collection);

				listState.LastFooterHeight = position.y - startPosition.y;
				listState.LastFooterHeight += 4;
				List.footerHeight = listState.LastFooterHeight;
			};

			newList.onRemoveCallback += (ReorderableList targetList) =>
			{
				if (targetList.index > -1)
				{
					int pageSize = GetItemsPerPage(listState.Collection);
					int pageStartIndex = GetPageDisplayRange(listState.Collection).x;
					listState.Collection.DeleteArrayElementAtIndex(pageStartIndex + targetList.index);
				}
			};

			newList.onAddCallback += (ReorderableList targetList) =>
			{
				if (DrawCustomAdd)
				{
					ListStates[collectionPath].AddingElement = !ListStates[collectionPath].AddingElement;
					if (ListStates[collectionPath].AddingElement)
						SerializationReflection.CallPrivateMethod(property, "ClearTemp");

				}
				else
					listState.Collection.InsertArrayElementAtIndex(listState.Collection.arraySize);
			};

			List = newList;
		}

		private float DrawAddArea(Rect position, SerializedProperty property, SerializedProperty collection)
		{
			DisplayState state = ListStates[CollectionDisplay.GetFullPath(collection)];
			if (!state.AddingElement || !collection.isExpanded)
				return 0;

			ReorderableList list = state.List.List;
			Rect startPosition = position;

			Rect labelPosition = position;

			labelPosition.height = EditorGUIUtility.singleLineHeight;
			UnityEngine.GUI.Label(labelPosition, new GUIContent("Add Element"), centeredLabelStyle);
			position.y += labelPosition.height;
			position.xMin += 20;
			position.xMax -= 20;

			if (DrawAddElement != null)
				position.y += DrawAddElement(position, property, collection);

			Rect addButtonPosition = position;
			addButtonPosition.xMax -= position.width * .55f;
			addButtonPosition.height = EditorGUIUtility.singleLineHeight;
			if (UnityEngine.GUI.Button(addButtonPosition, "Add"))
			{
				bool successful = true;
				try
				{
					SerializationReflection.CallPrivateMethod(property, "AddTemp");
				}
				catch (Exception e)
				{
					successful = false;
					EditorUtility.DisplayDialog("Cannot Add to Collection", e.InnerException.Message, "Okay");
				}

				if (successful)
					EndAddMode(state);
			}

			Rect cancelButtonPosition = position;
			cancelButtonPosition.xMin += position.width * .55f;
			cancelButtonPosition.height = EditorGUIUtility.singleLineHeight;
			if (UnityEngine.GUI.Button(cancelButtonPosition, "Cancel"))
			{
				SerializationReflection.CallPrivateMethod(property, "ClearTemp");
				EndAddMode(state);
				state.LastAddingHeight = 0;
				return 0;
			}

			position.y += addButtonPosition.height;
			position.y += 5;

			state.LastAddingHeight = position.y - startPosition.y;
			return state.LastAddingHeight;
		}

		private void EndAddMode(DisplayState listState)
		{
			listState.AddingElement = !listState.AddingElement;
			UnityEngine.GUI.FocusControl(null);
		}
#endif
	}
}
