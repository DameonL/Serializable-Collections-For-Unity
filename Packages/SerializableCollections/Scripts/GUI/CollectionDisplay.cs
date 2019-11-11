#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using System;
using UnityEngine;
using System.Text;

namespace SerializableCollections.GUI
{
	public class CollectionDisplay
	{
#if UNITY_EDITOR
		public static System.Collections.Generic.Dictionary<string, DrawerState> DrawerStates { get; private set; } = new System.Collections.Generic.Dictionary<string, DrawerState>();

		private static PlayModeStateChange playModeState;
		private static GUIStyle centeredLabelStyle = new GUIStyle(EditorStyles.boldLabel);
		private static GUIStyle headerStyle = new GUIStyle(EditorStyles.miniBoldLabel);

		static CollectionDisplay()
		{
			Selection.selectionChanged += () => DrawerStates.Clear();

			EditorApplication.hierarchyChanged += () =>
			{
				DrawerStates.Clear();
			};

			centeredLabelStyle.alignment = TextAnchor.UpperCenter;
			headerStyle.fontSize = 12;
			EditorApplication.playModeStateChanged -= OnPlayStateChanged;
			EditorApplication.playModeStateChanged += OnPlayStateChanged;
		}

		public static string GetFullPath(SerializedProperty property)
		{
			return property.serializedObject.targetObject.GetInstanceID() + property.propertyPath;
		}

		public static DrawerState GetDrawerState(SerializedProperty property)
		{
			string propertyPath = GetFullPath(property);
			DrawerState state;
			if (!DrawerStates.ContainsKey(propertyPath))
			{
				state = new DrawerState(property);
				DrawerStates.Add(GetFullPath(property), state);
				state.Property = property;
				state.Height = 0;
			}
			else
				state = DrawerStates[propertyPath];

			return state;
		}

		public Rect Render(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.serializedObject.targetObject == null)
				return position;

			if (playModeState == PlayModeStateChange.ExitingEditMode || playModeState == PlayModeStateChange.ExitingPlayMode)
				return position;

			DrawerState state = GetDrawerState(property);

			SerializedProperty collection = property.FindPropertyRelative("values");
			if (collection == null)
				Debug.Log(property.name);
			if (!ReorderableCollection.ListStates.ContainsKey(GetFullPath(collection)))
			{
				var newList = new ReorderableCollection(property, collection);
				newList.DrawHeaderAreaHandlers.Add(DrawTitle);

				newList.DrawElement += DrawElement;
				newList.DrawAddElement = DrawAddElement;
				newList.GetElementHeight = GetElementHeight;
				newList.Reorder += OnReorder;
			}

			state.Property = property;
			state.Height = 0;

			Rect startPosition = position;

			DisplayState listState = ReorderableCollection.ListStates[GetFullPath(collection)];
			Color cachedColor = UnityEngine.GUI.color;
			var settings = PackageSettings.Settings.GetListSettings(collection);

			if (settings != null && settings.Color != Color.white)
				UnityEngine.GUI.color = settings.Color;

			if (collection != null)
			{
				position = DrawCollection(position, property, collection, label);
			}
			state.Height = position.y - startPosition.y;
			UnityEngine.GUI.color = cachedColor;

			return position;
		}

		protected virtual Rect DrawCollection(Rect position, SerializedProperty property, SerializedProperty collection, GUIContent label)
		{
			EditorGUI.BeginProperty(position, new GUIContent(collection.displayName), collection);
			{
				Rect foldoutPos = position;
				foldoutPos.height = EditorGUIUtility.singleLineHeight;
				collection.isExpanded = EditorGUI.Foldout(foldoutPos, collection.isExpanded, label, true);

				position.y += EditorGUIUtility.singleLineHeight;

				DisplayState listState = ReorderableCollection.ListStates[GetFullPath(collection)];
				listState.Collection = collection;
				ReorderableList valuePairList = listState.List.List;
				valuePairList.serializedProperty = collection;
				if (collection.isExpanded && collection.isArray)
				{
					valuePairList.DoList(position);
					position.y += valuePairList.GetHeight();
				}
			}
			EditorGUI.EndProperty();
			return position;
		}

		private static void OnPlayStateChanged(PlayModeStateChange state)
		{
			playModeState = state;
			ReorderableCollection.ListStates.Clear();
		}

		public virtual float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (DrawerStates.ContainsKey(GetFullPath(property)))
				return DrawerStates[GetFullPath(property)].Height;

			return EditorGUI.GetPropertyHeight(property);
		}

		protected virtual float GetElementHeight(int index, SerializedProperty property, SerializedProperty collection)
		{
			if (collection != null)
				return EditorGUI.GetPropertyHeight(collection.GetArrayElementAtIndex(index));

			return 0;
		}

		protected virtual void OnReorder(SerializedProperty property, SerializedProperty collection, int oldIndex, int newIndex)
		{
		}

		protected virtual float DrawTitle(Rect position, SerializedProperty property, SerializedProperty collection)
		{
			Rect startPosition = position;
			var fieldValue = SerializationReflection.GetPathReference(collection);
			Type[] generics = fieldValue.GetType().GenericTypeArguments;
			StringBuilder typeArguments = new StringBuilder();
			for (var i = 0; i < generics.Length; i++)
			{
				typeArguments.Append(ObjectNames.NicifyVariableName(generics[i].Name));
				if (i < generics.Length - 1)
					typeArguments.Append(", ");
			}

			string nicePropertyType = ObjectNames.NicifyVariableName(property.type);
			GUIContent label = new GUIContent($"{nicePropertyType} ({typeArguments.ToString()})");
			position.height = EditorStyles.label.CalcHeight(label, position.width) + 5;

			EditorGUI.LabelField(position, label, headerStyle);

			position.y += position.height;

			return position.y - startPosition.y;
		}

		protected virtual void DrawElement(Rect position, int index, SerializedProperty property, SerializedProperty collection)
		{
			position.xMin += 10;
			position.yMax -= 5;
			SerializedProperty arrayElement = collection.GetArrayElementAtIndex(index);
			UnityEngine.GUI.SetNextControlName($"{GetFullPath(collection)}/{index}");
			float fieldHeight = EditorStyles.textField.fixedHeight;
			EditorStyles.textField.fixedHeight = position.height + 2;
			if ((arrayElement.propertyType != SerializedPropertyType.ObjectReference)
				&& (arrayElement.propertyType != SerializedPropertyType.String))
				EditorGUI.PropertyField(position, arrayElement, true);
			else
				EditorGUI.PropertyField(position, arrayElement, GUIContent.none, true);

			EditorStyles.textField.fixedHeight = fieldHeight;
		}

		protected virtual float DrawAddElement(Rect position, SerializedProperty property, SerializedProperty collection)
		{
			SerializedProperty value = property.FindPropertyRelative("value");
			if (value == null)
				return 0;

			position.height = EditorGUI.GetPropertyHeight(value);
			EditorGUI.PropertyField(position, value, true);
			return position.height;
		}

#endif
	}
}