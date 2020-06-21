using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Ur.Collections;
using Ur.Geometry;

namespace Cuneiform {

    public abstract class UIElement : IComparable<UIElement>, IUIElement {
        public UISystem UI { get; internal set; }
        internal Tree<UIElement> tree;

        private List<Slice> slices;

        internal UIElement Parent => tree.ParentOf(this);
        internal IEnumerable<UIElement> Children => tree.ChildrenOf(this);

        public Style Style { get; set; }

        public bool Enabled { get; set; } = true; // you can disable this later on.
        public bool Clickable { get; set; } = false; // non clickables are never interacted with. Useful for labels, groups...

        // used in uniquely comparing which element is on top. Remembers the order of each parent AND this member. The array count is equal to our generation.
        internal int[] Depths { get; set; }

        public float Width => LocalRect.W;
        public float Height => LocalRect.H;

        public Style EffectiveStyle {
            get {
                for (var i = this; i != null; i = i.Parent) if (i.Style != null) return i.Style;
                return null;
            }
        }

        public Rect LocalRect { get; set; }
        public Rect WorldRect {
            get {
                var p = Parent;
                var r = LocalRect;
                if (p == null) return r;
                return Rect.FromDimensions(
                    p.WorldRect.X0 + r.X0,
                    p.WorldRect.Y0 + r.Y0,
                    r.W,
                    r.H
                );
            }
        }


        public UIElement() {
            slices = new List<Slice>();
        }

        public void FitToParent(int margin = 0) {
            if (Parent == null) return;
            var wr = Parent.WorldRect;
            this.LocalRect = Rect.FromDimensions(margin, margin, wr.W - 2 * margin, wr.H - 2 * margin);
        }

        public virtual string Print => "Element";

        protected abstract void CreateSlices();

        protected void CleanupSlices() {
            foreach (var slice in slices) slice.Cleanup();
            slices.Clear();
        }

        internal virtual void Update() {
            foreach (var slice in slices) slice.Update();
        }

        internal void OnCreated() { 
            CreateSlices();
        }

        internal void OnRemoved() { CleanupSlices(); }

        public int CompareTo([AllowNull] UIElement other) {
            if (other == null) return 1;
            var n = Ur.Numbers.Min(Depths.Length, other.Depths.Length);
            for (var i = 0; i < n; i++) {
                if (Depths[i] > other.Depths[i]) return 1;
                else if (Depths[i] < other.Depths[i]) return -1;
            }
            return 0;
        }

        public void AddSlice(Slice s) {
            slices.Add(s);
            s.Init(this);
            foreach (var r in s.Renderables) UI.Canvas.Add(r);
        }
    }
}
