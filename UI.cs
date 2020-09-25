using Sargon.Assets;
using Sargon.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Ur.Collections;
using Sargon;
using Ur.Grid;

namespace Cuneiform {

    public class UISystem : State {
        Tree<UIElement> tree;

        internal Canvas Canvas { get; private set; }

        public float Zed { 
            get => Canvas.Zed;
            set => Canvas.Zed = value;
        }

        public UISystem() {
            InitializeTree();
            Canvas = new Canvas();
            allUnderCursor = new List<UIElement>();
        }

        protected override void Initialize() {
            base.Initialize();
            Register(Hooks.Frame, Update);
        }

        private void InitializeTree() {
            tree = new Tree<UIElement>();
            tree.ItemInserted += OnItemInserted;
            tree.ItemRemoved += OnItemRemoved;
            tree.TreeFlattened += OnTreeFlattened;
            var root = new Group();
            root.tree = tree;
            tree.Insert(root, null);
        }

        private void OnTreeFlattened(Tree<UIElement> tree) => DistributeDepths();
        private void OnItemRemoved(UIElement obj) => obj.OnRemoved();
        private void OnItemInserted(UIElement obj) => obj.OnCreated();

        void Update() {
            tree.GetRootElement().LocalRect = (0, 0, Context.Screen.Width, Context.Screen.Height);

            foreach (var item in tree.GetAllMembersInHierarchyOrder()) {
                item.Update();
            }

            MouseDelta = Context.Input.MousePosition - MousePosition;
            MousePosition = Context.Input.MousePosition;

            ProcessHighlights();
            UIFrame?.Invoke();
        }

        List<UIElement> allUnderCursor;

        UIElement cursorHighlighted;

        public event Action HighlightChanged;
        public event Action UIFrame;

        public Coords MousePosition;
        public Coords MouseDelta;

        public IUIElement Highlighted => cursorHighlighted;

        private void ProcessHighlights() {            
            var mousePos = (MousePosition.X, MousePosition.Y);
            allUnderCursor.Clear();
            allUnderCursor.AddRange(tree.GetAllMembersInHierarchyOrder().Where(member => member.Clickable && member.WorldRect.Contains(mousePos)));
            allUnderCursor.Sort();
            var prevCH = cursorHighlighted;
            cursorHighlighted = allUnderCursor.LastOrDefault();
            if (prevCH != cursorHighlighted) HighlightChanged?.Invoke();
        }

        private void DistributeDepths() {
            var list = new List<int>();

            var queue = new Queue<UIElement>();

            var root = tree.GetRootElement();
            root.Depths = new[] { 0 };
            queue.Enqueue(root);

            while (queue.Count > 0) {
                var item = queue.Dequeue();
                list.Clear();
                list.AddRange(item.Depths);
                var toCopy = list.ToArray();

                var childList = item.Children.ToList();

                for (var i = 0; i < childList.Count; i++) {
                    var child = childList[i];
                    child.Depths = new int[list.Count + 1];
                    toCopy.CopyTo(child.Depths, 0);
                    child.Depths[list.Count] = i;
                    queue.Enqueue(child);
                }
            }
        }

        public void Add(UIElement element, UIElement asChildOf = null) {
            if (asChildOf == null) asChildOf = tree.GetRootElement();

            if (!tree.ContainsItem(asChildOf)) throw new InvalidOperationException("Cannot insert to unknown parent");

            element.UI = this;
            element.tree = tree;
            tree.Insert(element, asChildOf);
        }

        public void RemoveFromUI(UIElement element) {
            tree.Remove(element);
        }
    }
}

