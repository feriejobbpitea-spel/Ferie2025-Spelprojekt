using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;

namespace EqualchanceGames.Tools.AutoTranslate.Windows
{
    public class AutoTranslate_EditorWindow : EditorWindow
    {
        private const string k_WindowTitle = "Auto Translate for Unity Localization";

        // Dummy datatyper, byt ut mot dina egna typer om du har
        private List<SharedStringTable> _sharedStringTables = new List<SharedStringTable>();
        private List<Locale> _locales = new List<Locale>();
        private CheckListGUI _checkListStringTable;
        private CheckListGUI _checkListLanguages;
        private DropdownLanguages _dropdownLanguages;
        private ITranslateApi translator;
        private bool _isErrorConnection = false;

        [MenuItem("Auto Localization/Auto Translate for String Tables")]
        public static void ShowWindow()
        {
            var window = GetWindow<AutoTranslate_EditorWindow>(false, k_WindowTitle);
            window.titleContent = new GUIContent(k_WindowTitle);
            window.Show();
        }

        private void OnEnable()
        {
            // Initiera listor med dummy-data eller fr�n dina faktiska data
            _sharedStringTables = LoadSharedStringTables();
            _locales = LoadLocales();

            _checkListStringTable = new CheckListGUI(_sharedStringTables.Select(t => t.TableCollectionName).ToList());
            _checkListLanguages = new CheckListGUI(_locales.Select(l => l.name).ToList());

            _checkListStringTable.Width = 100;
            _checkListStringTable.Height = 300;

            _checkListLanguages.Height = 300;
            _checkListLanguages.MinHeight = 200;

            _dropdownLanguages = new DropdownLanguages(_locales.Select(l => l.LocaleName).ToList());
            _dropdownLanguages.UpdateSelected += DropdownLanguages_UpdateSelected;

            _isErrorConnection = CheckInternetConnection();

            translator = FactoryTranslateApi.GetTranslateApi("default");
        }

        private void DropdownLanguages_UpdateSelected(string selectedName)
        {
            Debug.Log("Valt spr�k: " + selectedName);
        }

        private void OnGUI()
        {
            GUILayout.Label(k_WindowTitle, EditorStyles.boldLabel);

            if (_isErrorConnection == false)
            {
                EditorGUILayout.HelpBox("Ingen internetanslutning.", MessageType.Error);
            }

            GUILayout.Label("V�lj tabeller att �vers�tta:");
            _checkListStringTable?.DrawButtons();

            GUILayout.Space(10);

            GUILayout.Label("V�lj spr�k:");
            _checkListLanguages?.DrawButtons();

            GUILayout.Space(20);

            if (GUILayout.Button("�vers�tt"))
            {
                Debug.Log("�vers�ttningsknappen trycktes.");
                // H�r kan du anropa din �vers�ttningsmetod senare
            }
        }

        // Korrigerad LoadLocales-metod som anv�nder ScriptableObject.CreateInstance
        private List<Locale> LoadLocales()
        {
            var english = ScriptableObject.CreateInstance<Locale>();
            english.name = "English";
            english.LocaleName = "English";

            var swedish = ScriptableObject.CreateInstance<Locale>();
            swedish.name = "Swedish";
            swedish.LocaleName = "Swedish";

            return new List<Locale> { english, swedish };
        }

        private List<SharedStringTable> LoadSharedStringTables()
        {
            // H�r ska du h�mta dina faktiska string tables fr�n projektet
            return new List<SharedStringTable>
            {
                new SharedStringTable() { TableCollectionName = "ExampleTable1" },
                new SharedStringTable() { TableCollectionName = "ExampleTable2" }
            };
        }

        private bool CheckInternetConnection()
        {
            // Dummy: Returnera alltid true h�r, byt ut mot riktig check
            return true;
        }

        // Dummy-klasser (byt ut mot dina faktiska klasser!)

        private class SharedStringTable
        {
            public string TableCollectionName;
        }

        private class DropdownLanguages
        {
            public event Action<string> UpdateSelected;
            private List<string> _options;
            private int _selectedIndex = 0;

            public DropdownLanguages(List<string> options)
            {
                _options = options ?? new List<string>();
            }

            public void Draw()
            {
                int newIndex = EditorGUILayout.Popup("Language", _selectedIndex, _options.ToArray());
                if (newIndex != _selectedIndex)
                {
                    _selectedIndex = newIndex;
                    UpdateSelected?.Invoke(_options[_selectedIndex]);
                }
            }

            public string Selected => _options.ElementAtOrDefault(_selectedIndex) ?? "";
        }

        private class CheckListGUI
        {
            private List<string> _items;
            private Dictionary<string, bool> _checked = new Dictionary<string, bool>();

            public int Width = 100;
            public int Height = 300;
            public int MinHeight = 100;

            public CheckListGUI(List<string> items)
            {
                _items = items ?? new List<string>();
                foreach (var item in _items)
                    _checked[item] = false;
            }

            public void DrawButtons()
            {
                if (_items == null) return;

                EditorGUILayout.BeginVertical(GUILayout.Width(Width), GUILayout.Height(Height));
                foreach (var item in _items)
                {
                    _checked[item] = EditorGUILayout.Toggle(item, _checked[item]);
                }
                EditorGUILayout.EndVertical();
            }
        }

        private interface ITranslateApi
        {
            // Dummy interface, implementera dina metoder h�r
        }

        private static class FactoryTranslateApi
        {
            public static ITranslateApi GetTranslateApi(string service)
            {
                // Returnera dummy objekt eller din riktiga implementation
                return null;
            }
        }
    }
}
