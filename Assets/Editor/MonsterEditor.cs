using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MonsterEditor : EditorWindow
{

    VisualElement _detailDisplayArea; // 정보패널 디스플레이
    VisualTreeAsset _monsterDetailUxml;

    ScrollView scrollView;

    StyleSheet _monsterDetailUss;

    EditorSetting _EditorSettings;


    readonly string PATH = $"Assets/Resources/Monster";

    private static ScriptableObject _selectedMonsterType;

    public static ScriptableObject SelectedMonsterType
    {
        get => _selectedMonsterType;
        set => _selectedMonsterType = value;
    }

    public static VisualElement _rightPanel;

    [MenuItem("에디터/몬스터에디터")]
    public static void ShowWindow()
    {
        var window = GetWindow<MonsterEditor>("몬스터 에디터");
    }
    [OnOpenAsset]
    public static bool OnOpenAsset( int instanceID, int line )
    {
        if ( Selection.activeObject is MonsterData data )
        {
            ShowWindow();
            return true;
        }
        return false;
    }

    private void CreateGUI()
    {
        InitializeSetting();

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

        ///루트 레이아웃에 다 추가
        rootVisualElement.Add(leftPanel);
        rootVisualElement.Add(_detailDisplayArea);
        rootVisualElement.Add(_rightPanel);
        leftPanel.Add(AddMonsterButton);

        UpdateRightPanel();
    }
    void InitializeSetting()
    {
        _EditorSettings = EditorSetting.GetOrCreateSettings();
        _EditorSettings.behaviourTreeXml.CloneTree(rootVisualElement);
        rootVisualElement.styleSheets.Add(_EditorSettings.behaviourTreeStyle);

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

        var scriptableObjDatas = EditorExtension.LoadAssets<ScriptableObject>(PATH);

        var monsterButtonContainer = new VisualElement();
        monsterButtonContainer.style.flexDirection = FlexDirection.Column; // 세로로 생성
        monsterButtonContainer.style.alignItems = Align.Center; // 가운데 정렬
        foreach ( var data in scriptableObjDatas )
        {
            CreateButton($"{data.name}", () =>
            {
                _selectedMonsterType = data;
                UpdateRightPanel();
            }, (150, 20), monsterButtonContainer);
        }
        scrollView.Add(monsterButtonContainer);

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

    #region Display Graph Panel
    void DisplayBehaviourTree( VisualElement panel, ScriptableObject data = null )
    {
        panel.Clear();

        if ( data == null )
            data = SelectData();
        if ( data == null )
        {
            panel.Add(new Label("몬스터를 선택하거나 새로 생성해주세요."));
            return;
        }

        var _btGraph = new BehaviourTreeView();
        panel.Add(_btGraph);

    }
    #endregion

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

    public class BehaviourTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }
        public ScriptTemplate [] scriptFileAssets = {

            new ScriptTemplate{ templateFile=EditorSetting.GetOrCreateSettings().scriptTemplateActionNode, defaultFileName="NewActionNode.cs", subFolder="Actions" },
            new ScriptTemplate{ templateFile=EditorSetting.GetOrCreateSettings().scriptTemplateCompositeNode, defaultFileName="NewCompositeNode.cs", subFolder="Composites" },
            new ScriptTemplate{ templateFile=EditorSetting.GetOrCreateSettings().scriptTemplateDecoratorNode, defaultFileName="NewDecoratorNode.cs", subFolder="Decorators" },
        };
        VisualTreeAsset _nodeDetailUxml;
        Dictionary<string, Node> _nodeMap = new();
        BehaviourTreeAsset _currentTree;
        public BehaviourTreeView()
        {
            style.flexGrow = 1;
            style.flexShrink = 1;

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var toolbar = new Toolbar();
            Add(toolbar);

            toolbar.style.position = Position.Absolute;
            toolbar.style.top = 0;
            toolbar.style.left = 0;
            toolbar.style.right = 0;
            toolbar.style.height = 20;
            toolbar.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.8f);

            // 저장 버튼 추가
            CreateButton("저장", SaveCurrentGraphData, (70, 20), toolbar);


            var grid = new GridBackground();

            Insert(0, grid);
            grid.StretchToParentSize();

            ApplyUSS();

            RegistKeyEvent();

            InitializeGraph();

            this.graphViewChanged += OnChangeConnectNode;

        }

        private GraphViewChange OnChangeConnectNode( GraphViewChange graphViewChange )
        {
            if( graphViewChange.edgesToCreate != null )
            {
                EdgeToCreateUpdate(graphViewChange);
            }

            if(graphViewChange.elementsToRemove != null )
            {
                EdgeToRemoveUpdate(graphViewChange);
            }

            return graphViewChange;
        }

        void EdgeToCreateUpdate( GraphViewChange g)
        {
            List<Edge> temp = new();

            foreach ( var edge in g.edgesToCreate )
            {
                NodeView parentNodeView = edge.output.node as NodeView;
                NodeView childNodeView = edge.input.node as NodeView;

                if ( parentNodeView != null && childNodeView != null )
                {
                    Node parentNodeData = parentNodeView.node;
                    Node childNodeData = childNodeView.node;

                    if ( IsCircularReference(parentNodeData, childNodeData) )
                    {
                        Debug.LogWarning($"it is circulation ref can't add '{childNodeData.name}' to '{parentNodeData.name}'");
                        temp.Add(edge);
                        continue; // 다음 엣지
                    }

                    //단일 자식 제한
                    if ( parentNodeData is DecoratorNode && parentNodeData.ChildrenGUIDs.Count >= 1 )
                    {
                        Debug.LogWarning($"DecoratorNode Can't add to '{parentNodeData.name}'");
                        g.edgesToCreate.Remove(edge);
                        continue;


                    }
                    if ( parentNodeData != null && childNodeData != null )
                    {
                        if ( !parentNodeData.ChildrenGUIDs.Contains(childNodeData.guid) )
                        {
                            parentNodeData.ChildrenGUIDs.Add(childNodeData.guid);
                            EditorUtility.SetDirty(parentNodeData);
                            Debug.Log($"Connect edge: {parentNodeData.name} -> {childNodeData.name}");
                        }
                    }
                }
            }

            //순환 참조 리스트 삭제
            foreach(var e in temp )
            {
                g.edgesToCreate.Remove(e);
            }

        }

        void EdgeToRemoveUpdate( GraphViewChange g)
        {
            foreach ( var element in g.elementsToRemove )
            {
                if ( element is Edge edge )
                {
                    NodeView parentNodeView = edge.output.node as NodeView;
                    NodeView childNodeView = edge.input.node as NodeView;

                    if ( parentNodeView != null && childNodeView != null )
                    {
                        Node parentNodeData = parentNodeView.node;
                        Node childNodeData = childNodeView.node;

                        if ( parentNodeData != null && childNodeData != null )
                        {
                            if ( parentNodeData.ChildrenGUIDs.Remove(childNodeData.guid) )
                            {
                                EditorUtility.SetDirty(parentNodeData);
                                Debug.Log($"Remove Edge: {parentNodeData.name} -x-> {childNodeData.name}");
                            }
                        }
                    }
                }
                else if ( element is NodeView nodeView )
                {
                    if ( nodeView.node != null && _currentTree!= null )
                    {
                        AssetDatabase.RemoveObjectFromAsset(nodeView.node);
                        EditorUtility.SetDirty(_currentTree); 
                        Debug.Log($"Delete Node: {nodeView.node.name}");
                    }
                    _nodeMap?.Remove(nodeView.node.guid);
                }
            }
        }

        public void LoadGraph( BehaviourTreeAsset loadedAsset )
        {
            ClearGraphView(); // 기존 그래프 뷰 내용 초기화

            if ( loadedAsset == null )
            {
                Debug.LogError("로드할 BehaviorTreeAsset이 null입니다.");
                return;
            }

            _currentTree = loadedAsset; 
            _nodeMap = new Dictionary<string, Node>(); 


            var allNodesInAsset = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(loadedAsset))
                                   .OfType<Node>() // Node 타입을 상속받는 모든 객체 필터링
                                   .ToList();

            foreach ( var node in allNodesInAsset )
            {
                if ( node != null && !string.IsNullOrEmpty(node.guid) )
                {
                    _nodeMap [node.guid] = node;
                }
            }

            if ( loadedAsset.rootNode != null && _nodeMap.ContainsKey(loadedAsset.rootNode.guid) )
            {
                DrawNodeViewsRecursive(loadedAsset.rootNode);
            }
            else if ( loadedAsset.rootNode == null && allNodesInAsset.Any() )
            {

                Debug.LogWarning("BehaviorTreeAsset에 rootNode가 설정되어 있지 않지만, 다른 노드가 존재합니다. 첫 번째 노드를 루트로 간주합니다.");
                DrawNodeViewsRecursive(allNodesInAsset.First());
            }
            else
            {
                Debug.Log("로드된 BehaviorTreeAsset에 노드가 없습니다. 새 트리를 만드세요.");
            }

            ConnectEdgesFromData();
        }

        private void DrawNodeViewsRecursive( Node currentNode )
        {
            // 이미 그려진 노드 뷰이거나, 유효하지 않은 노드 데이터면 중복 생성 방지
            if ( currentNode == null || GetNodeByGuid(currentNode.guid) != null ) return;

            NodeView nodeView = CreateNodeView(currentNode);

            foreach ( string childGuid in currentNode.ChildrenGUIDs )
            {
                if ( _nodeMap.TryGetValue(childGuid, out Node childNodeData) )
                {
                    DrawNodeViewsRecursive(childNodeData);
                }
                else
                {
                    Debug.LogWarning($"자식 노드 GUID '{childGuid}'못찾겠음 ");
                }
            }
        }

        private NodeView GetNodeViewByGuid( string guid )
        {
            return this.contentViewContainer.Query<NodeView>().Where(v => v.node.guid == guid).First();
        }

        private void ConnectEdgesFromData()
        {
            foreach ( var nodeView in this.contentViewContainer.Query<NodeView>().ToList() )
            {
                Node parentNodeData = nodeView.node;
                if ( parentNodeData == null ) continue;

                foreach ( string childGuid in parentNodeData.ChildrenGUIDs )
                {

                    NodeView childNodeView = GetNodeViewByGuid(childGuid);

                    if ( childNodeView != null )
                    {
                        Port outputPort = nodeView.outputContainer.Query<Port>().First();
                        Port inputPort = childNodeView.inputContainer.Query<Port>().First();

                        if ( outputPort != null && inputPort != null )
                        {

                            if ( !outputPort.Contains(inputPort) )
                            {
                                Edge edge = outputPort.ConnectTo(inputPort); 
                                AddElement(edge); 
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"엣지 연결을 위한 자식 노드 뷰 '{childGuid}'를 찾을 수 없습니다.");
                    }
                }
            }
        }


        private void ClearGraphView()
        {

            this.DeleteElements(this.graphElements.ToList());
            _nodeMap?.Clear(); 
            _currentTree = null;  
        }
        void SaveCurrentGraphData()
        {

            foreach ( var nodeData in _nodeMap.Values )
            {
                Debug.Log($"save {nodeData.Type}");
                EditorUtility.SetDirty(nodeData);
            }

            EditorUtility.SetDirty(_currentTree);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Save Graph '{_currentTree.name}.asset' ");

        }
        void ApplyUSS()
        {
            StyleSheet bgSS = EditorSetting.GetOrCreateSettings().grapthViewBackgroundStyle;
            if ( bgSS != null )
                styleSheets.Add(bgSS);
        }
        NodeView CreateNodeView( Node node)
        {
            
            NodeView nodeView = new(node);

            nodeView.SetNodeName(node.nodeName);

            //TODO : 재구성 할 필요 있을듯
            switch (node.nodeName)
            {
                case ROOT:
                    nodeView.SetPosition(new Rect(0, 0, 150, 50));
                    _currentTree.rootNode = node;
                    AssetDatabase.AddObjectToAsset(node, _currentTree);
                    EditorUtility.SetDirty(_currentTree.rootNode);
                    EditorUtility.SetDirty(_currentTree);
                    AssetDatabase.SaveAssets();

                    AssetDatabase.Refresh();
                    break;
            }


            nodeView.onSelectedNode += ( _ ) =>
            {
                _rightPanel.Clear(); // 기존UI 제거

                _rightPanel.Add(_nodeDetailUxml.CloneTree());

                UpdateNodeContents(nodeView, _rightPanel);

            };

            nodeView.RegisterCallback<GeometryChangedEvent, NodeView>(UpdateNodeOnDrag, nodeView);
            nodeView.SetContainerColor(Color.gray);

            AddElement(nodeView);

            return nodeView;
        }

        void UpdateNodeOnDrag( GeometryChangedEvent evt, NodeView nodeView )
        {
            if ( evt.oldRect.position == evt.newRect.position ) return;

            Node node = nodeView.node;
            if ( node != null )
            {
                node.position = nodeView.GetPosition().position;
                EditorUtility.SetDirty( node ); 
            }
        }


        void UpdateNodeContents( NodeView nodeView, VisualElement panel )
        {
            var nameLabel = panel.Q<Label>("NodeNameLabel");
            var nodeType = panel.Q<DropdownField>("NodeType");
            var conditionList = panel.Q<DropdownField>("ConditionList");
            var actionList = panel.Q<DropdownField>("ActionList");

            //TODO : 컨디션,액션 리스트는 노드에 맞는 컨디션이 추가되도록 나중에 데이터화 
            List<string> nodeTypes = new() { typeof(SelectorNode).Name, typeof(SequenceNode).Name, typeof(DecoratorNode).Name };
            List<string> conditions = new() { "Always", "In Range", "Out Range" };
            List<string> actions = new() { "Idle", "Patrol", "Trace", "Attack", "Return", "Death" };

            nodeType.choices = nodeTypes;
            conditionList.choices = conditions;
            actionList.choices = actions;



            nodeType.value = nodeView.node.GetType().Name;
            nameLabel.text = $"{nodeView.title}";
        }
        void RegistKeyEvent()
        {
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }
        void OnKeyDown( KeyDownEvent evt )
        {
            if ( evt.ctrlKey )
            {
                Debug.Log("Press Ctrl");
                switch ( evt.keyCode )
                {
                    case KeyCode.C:
                        CopySelect();
                        break;
                    case KeyCode.V:
                        PasteSelect();
                        break;
                }
            }
        }
        void CopySelect()
        {
            foreach ( var ele in selection )
            {
                if ( ele is NodeView node )
                {
                    Debug.Log($"{node.name} copy");
                }

            }
        }
        void PasteSelect()
        {
            foreach ( var ele in selection )
            {
                if ( ele is NodeView node )
                {
                    Debug.Log($"{node.name} paste");
                }

            }
        }
        const string ROOT = "ROOT";
        public void InitializeGraph()
        {
            _nodeDetailUxml = EditorSetting.GetOrCreateSettings().nodeXml;

            var assetPath = $"Assets/Resources/MonsterPattern/{_selectedMonsterType}.asset";

            var asset =  AssetDatabase.LoadAssetAtPath<BehaviourTreeAsset>(assetPath);
            if(asset != null )
            {
                _currentTree = asset;
                LoadGraph(asset);
            }
            else
            {

                var newBTAsset = CreateInstance<BehaviourTreeAsset>();
                AssetDatabase.CreateAsset(newBTAsset, assetPath);
                _currentTree = AssetDatabase.LoadAssetAtPath<BehaviourTreeAsset>(assetPath);

                var node = CreateStartNode();
                CreateNodeView(node);
                _nodeMap.Add(node.guid, node);
            }

        }
        private bool IsCircularReference( Node potentialParent, Node potentialChild )
        {
            if ( potentialParent.guid == potentialChild.guid ) return true;

            Node current = potentialParent;
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(current);

            //bfs
            Queue<Node> q = new Queue<Node>();
            HashSet<string> visited = new HashSet<string>();

            q.Enqueue(potentialChild);
            visited.Add(potentialChild.guid);

            while ( q.Count > 0 )
            {
                Node node = q.Dequeue();
                if ( node.guid == potentialParent.guid )
                {
                    return true;
                }

                foreach ( string childGuid in node.ChildrenGUIDs )
                {
                    if ( _nodeMap.TryGetValue(childGuid, out Node childNode) && !visited.Contains(childGuid) )
                    {
                        q.Enqueue(childNode);
                        visited.Add(childGuid);
                    }
                }
            }
            return false;
        }
        public NodeView FindNodeView( Node node )
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }
        public struct ScriptTemplate
        {
            public TextAsset templateFile;
            public string defaultFileName;
            public string subFolder;
        }
        Node CreateStartNode()
        {

            Node node = CreateInstance<SelectorNode>();
            node.nodeName = ROOT;

            return node;
        }

        public override List<Port> GetCompatiblePorts( Port startPort, NodeAdapter nodeAdapter )
        {
            return ports.ToList()!.Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node &&
            endPort.portType == startPort.portType).ToList();
        }

        public override void BuildContextualMenu( ContextualMenuPopulateEvent evt )
        {
            evt.menu.AppendAction($"{Node.NodeType.Selector.ToString()}", ( d => OnContextMenuNodeCreate(d, Node.NodeType.Selector) ));
            evt.menu.AppendAction($"{Node.NodeType.Sequence.ToString()}", ( d => OnContextMenuNodeCreate(d, Node.NodeType.Sequence) ));
            evt.menu.AppendAction($"{Node.NodeType.Decorator.ToString()}", ( d => OnContextMenuNodeCreate(d, Node.NodeType.Decorator) ));
        }

        void OnContextMenuNodeCreate( DropdownMenuAction d, Node.NodeType type )
        {
            Node node = CreateInstance<Node>();
            switch ( type )
            {
                case Node.NodeType.Selector:
                    node = CreateInstance<SelectorNode>();
                    break;
                case Node.NodeType.Sequence:
                    node = CreateInstance<SequenceNode>();
                    break;
                case Node.NodeType.Decorator:
                    node = CreateInstance<DecoratorNode>();
                    break;
            }


            var mousePos = d.eventInfo.localMousePosition;
            var localPos = contentContainer.WorldToLocal(mousePos);
            node.position = localPos;

            node.nodeName = d.name;
            CreateNodeView(node);
            _nodeMap.Add(node.guid, node);
            AssetDatabase.AddObjectToAsset(node, _currentTree);
        }
    }

    ScriptableObject SelectData()
    {
        var loadedAsset = AssetDatabase.LoadAssetAtPath(Path.Combine(PATH, $"{_selectedMonsterType.name}.asset"), typeof(ScriptableObject));
        ScriptableObject infoData = loadedAsset as ScriptableObject;
        return infoData;

    }
    public static Button CreateButton( string _text, Action func, (float x, float y) size, VisualElement panel )
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

