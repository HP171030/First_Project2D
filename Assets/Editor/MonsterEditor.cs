using System;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterEditor : EditorWindow
{
    ScriptableObject _selectedMonsterType; // 현재 선택된 몬스터 타입을 저장
    VisualElement _rightPanel; // 우측 패널 참조
    VisualElement _detailDisplayArea; // 정보패널 디스플레이
    ScrollView scrollView;
    [SerializeField] VisualTreeAsset _monsterDetailUxml;
    [SerializeField] StyleSheet _monsterDetailUss;


    readonly string PATH = $"Assets/Resources/Monster";

    [MenuItem("에디터/몬스터에디터")]
    public static void ShowWindow()
    {
        var window = GetWindow<MonsterEditor>("몬스터 에디터");
        window._monsterDetailUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MonsterDetail.uxml");
        window._monsterDetailUss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/MonsterDetail.uss");
    }


    private void CreateGUI()
    {
        rootVisualElement.style.flexDirection = FlexDirection.Row;
        ///몬스터 리스트 레이아웃
        var leftPanel = new VisualElement();
        leftPanel.style.width = 200;
        leftPanel.style.borderBottomColor = Color.gray;
        leftPanel.style.borderBottomWidth = 1;
        leftPanel.style.flexShrink = 0;

        scrollView = new ScrollView();
        scrollView.style.flexGrow = 1f;
        leftPanel.Add(scrollView);

        var AddMonsterButton = new Button(() =>
        {
            CreateNewPrototype(typeof(MonsterData));
            UpdateLeftPanel();
        })
        { text = "Add Monster" };

        UpdateLeftPanel();

        ///
        /// 우측 레이아웃
        _rightPanel = new VisualElement();
        _rightPanel.style.width = 200;
        _rightPanel.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
        _rightPanel.style.paddingBottom = new StyleLength(10);
        _rightPanel.style.paddingLeft = new StyleLength(10);
        _rightPanel.style.paddingRight = new StyleLength(10);
        _rightPanel.style.paddingTop = new StyleLength(10);
        _rightPanel.style.flexDirection = FlexDirection.Column;
        _rightPanel.style.alignItems = Align.Center;

        _detailDisplayArea = new VisualElement();
        _detailDisplayArea.style.flexGrow = 1; // 남은 공간을 차지하도록
        _detailDisplayArea.style.marginTop = 10;
        _detailDisplayArea.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f); // 상세 정보 영역 배경색

        /// 

        rootVisualElement.Add(leftPanel);
        rootVisualElement.Add(_detailDisplayArea);
        rootVisualElement.Add(_rightPanel);
        leftPanel.Add(AddMonsterButton);

        UpdateRightPanel();
    }

    #region Update Left Panel
    private void UpdateLeftPanel()
    {
        scrollView.Clear();

        var label = CreateLabel("Monster List", FontStyle.Bold, 20, 10, Color.red, scrollView);
        label.style.marginTop = 10;
        label.style.unityTextAlign = TextAnchor.UpperCenter;

        var separator = new VisualElement();
        separator.style.height = 1;
        separator.style.backgroundColor = Color.gray;
        separator.style.marginTop = 5;
        separator.style.marginBottom = 20;
        scrollView.Add(separator);

        var guids = AssetDatabase.FindAssets("t:ScriptableObject", new [] { PATH });
        foreach ( var guid in guids )
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MonsterData asset = AssetDatabase.LoadAssetAtPath<MonsterData>(path);

            var button = new Button(() =>
            {
                Debug.Log($"Clicked {asset.name}");
                _selectedMonsterType = asset;
                UpdateRightPanel();
            })
            {
                text = asset.name
            };
            scrollView.Add(button);
        }
    }
    #endregion

    #region Update Right Panel
    private void UpdateRightPanel()
    {
        _rightPanel.Clear();

        if ( _selectedMonsterType == null )
        {
            _rightPanel.Add(new Label("몬스터를 먼저 선택해주세요"));
            return;
        }
        var selectedData = _selectedMonsterType as MonsterData;

        //몬스터 이름 라벨
        CreateLabel($"{selectedData.name}", FontStyle.Bold, 20, 15, Color.yellow, _rightPanel);

        //정보버튼
        CreateButton("정보", () =>
        {
            DisplayPrototypeDetails();
        },
        (150, 20), _rightPanel);

        //BT
        CreateButton("행동 패턴", () =>
        {
            DisplayBehaviourTree(_detailDisplayArea);
        },
        (150, 20), _rightPanel);

    }
    #endregion

    #region Display Detail panel
    void DisplayPrototypeDetails( ScriptableObject data = null )
    {
        _detailDisplayArea.Clear();

        if ( data == null )
            data = SelectData();
        if ( data == null )
        {
            _detailDisplayArea.Add(new Label("몬스터를 선택하거나 새로 생성해주세요."));
            return;
        }

        _detailDisplayArea.Add(new Label($"{data.name}")
        {
            style = {
                unityFontStyleAndWeight = FontStyle.Bold,
                marginTop = 5,
                marginBottom = 5,
                color = Color.cyan
            }
        });

        var serializedObject = new SerializedObject(data);
        var iterator = serializedObject.GetIterator();
        bool enterChildren = true;

        while ( iterator.NextVisible(enterChildren) )
        {
            enterChildren = false;
            var propertyField = new PropertyField(iterator);
            propertyField.Bind(serializedObject);
            _detailDisplayArea.Add(propertyField);
        }

        var nameField = new TextField("파일 이름:");
        nameField.value = data.name;
        nameField.RegisterValueChangedCallback(evt =>
        {
            data.name = evt.newValue;
            EditorUtility.SetDirty(data);
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(data), evt.newValue);
            AssetDatabase.SaveAssets();
            UpdateRightPanel();
        });
        _detailDisplayArea.Insert(1, nameField);

        var saveButton = new Button(() =>
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            Debug.Log($"{data.name} 저장 완료");
            UpdateLeftPanel();
        })
        {
            text = "저장"
        };
        _detailDisplayArea.Add(saveButton);
    }
    #endregion
    void DisplayBehaviourTree( VisualElement panel, ScriptableObject data = null)
    {
        panel.Clear();

        if ( data == null )
            data = SelectData();
        if ( data == null )
        {
            panel.Add(new Label("몬스터를 선택하거나 새로 생성해주세요."));
            return;
        }

        var _btGraph = new BehaviourTree();
        panel.Add(_btGraph);

    }

    #region Create New Monster Data
    private void CreateNewPrototype( Type monsterType )
    {
        string path = EditorUtility.SaveFilePanel($"{monsterType.Name} 추가", PATH, $"{monsterType.Name}", "asset");
        int index = path.IndexOf("Assets");
        string relativePath = "";
        if ( index >= 0 )
        {
            relativePath = path.Substring(index).Replace("\\", "/");
        }

        if ( !string.IsNullOrEmpty(relativePath) )
        {
            ScriptableObject newPrototype = CreateInstance(monsterType);
            if ( newPrototype != null )
            {
                AssetDatabase.CreateAsset(newPrototype, relativePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"새 프로토타입 생성됨: {path}");

                _selectedMonsterType = newPrototype;
                DisplayPrototypeDetails(newPrototype);
                UpdateRightPanel();
            }
            else
            {
                Debug.LogError($"'{monsterType.Name}' 타입의 인스턴스를 생성할 수 없습니다. 'Monster'는 ScriptableObject를 상속해야 합니다.");
            }
        }
    }
    #endregion

    public class BehaviourTree : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTree, UxmlTraits> { }
        public BehaviourTree()
        {
            Debug.Log("BT 호출");

            style.flexGrow = 1;
            style.flexShrink = 1;

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());


            var grid = new GridBackground();

            Insert(0, grid);
            grid.StretchToParentSize();

            StyleSheet ss = (StyleSheet)EditorGUIUtility.Load("GridBackground_uss.uss");
            if ( ss != null )
            {
                Debug.Log("스타일시트 추가");
                styleSheets.Add(ss);
            }
            else
            {
                Debug.LogWarning("Editor_uss.uss stylesheet not found. Ensure it's in a Resources folder or specified path.");
            }

        }
    }

    ScriptableObject SelectData()
    {
        var loadedAsset = AssetDatabase.LoadAssetAtPath(Path.Combine(PATH, $"{_selectedMonsterType.name}.asset"), typeof(ScriptableObject));
        ScriptableObject infoData = loadedAsset as ScriptableObject;
        return infoData;

    }
    Button CreateButton( string _text, Action func, (float x, float y) size, VisualElement panel )
    {
        var _button = new Button(func)
        {
            text = _text
        };
        _button.style.width = size.x;
        _button.style.height = size.y;
        panel.Add(_button);
        return _button;
    }
    Label CreateLabel( string text, FontStyle fontStyle, int fontSize, int marginBottom, Color color, VisualElement panel )
    {
        var label = new Label(text);

        label.style.unityFontStyleAndWeight = fontStyle;
        label.style.fontSize = fontSize;
        label.style.marginBottom = marginBottom;
        label.style.color = color;

        panel.Add(label);
        return label;
    }
}