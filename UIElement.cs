﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Ur.Collections;
using Ur.Geometry;

namespace Cuneiform {

    public abstract class UIElement : IComparable<UIElement>, IUIElement {

        public UISystem UI { get; internal set; }
        internal Tree<UIElement> tree;

        private List<Slice> slices;
        private List<UIBehaviour> behaviours;

        internal UIElement Parent => tree.ParentOf(this);
        internal IEnumerable<UIElement> Children => tree.ChildrenOf(this);

        public Style Style { get; set; }

        internal bool IsDrawRoot => this is IHasRenderTexture hrt ? hrt.ToTexture : Parent == null;

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

        public void AttachBehaviour(UIBehaviour behaviour) {
            behaviour.AttachTo(this);
            this.behaviours.Add(behaviour);
        }

        public void RemoveBehaviour(UIBehaviour behaviour) {
            if (behaviours.Remove(behaviour)) behaviour.Detach();
        }

        Rect localRect;

        public Rect LocalRect { 
            get => localRect;
            set {
                localRect = value;
                OnLocalRectUpdated();
            }
        }

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

        /// <summary>
        /// Draw Rect is similar to world rect, but returns coordinates relative to the DRAW ROOT.
        /// 
        /// </summary>
        public Rect DrawRect { get { 
            // to get local coords relative to the draw root just add all the local coords OF ALL THE ELEMENTS UP THE CHAIN
            // UP TO DRAW ROOT, BUT NOT INCLUDING DRAW ROOT
            Vector2 xy = (0,0);
            for (var e = this; e != null; e = e.Parent) {
                if (e.IsDrawRoot) break;
                xy += (e.localRect.X0, e.localRect.Y0);
            }            
            return Rect.FromDimensions(xy.x, xy.y, localRect.W, localRect.H);
        } }

        protected virtual void OnLocalRectUpdated() {

        }

        public UIElement() {
            slices = new List<Slice>();
        }

        /// <summary> Modifies the rect of a UI element so that it fits the parent, with margin (defaults to zero) </summary>
        /// <param name="margin"></param>
        public void FitToParent(int margin = 0) {
            if (Parent == null) return;
            var wr = Parent.WorldRect;
            this.LocalRect = Rect.FromDimensions(margin, margin, wr.W - 2 * margin, wr.H - 2 * margin);
        }

        public virtual string Print => "Element";

        protected void CleanupSlices() {
            foreach (var slice in slices) slice.Cleanup();
            slices.Clear();
        }

        internal virtual void Update() {
            foreach (var slice in slices) slice.Update();
        }

        internal void OnCreated() { 
            behaviours = new List<UIBehaviour>();
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

        IHasRenderTexture FindDrawRoot() {
            for (var e = this; e != null; e = e.Parent)
                if (e is IHasRenderTexture ihrt && ihrt.ToTexture) return ihrt;
            
            return null;
        }

        public void AddSlice(Slice s) {
            slices.Add(s);
            s.Init(this);

            var targetCanvas = FindDrawRoot()?.Canvas ?? UI.Canvas;
            foreach (var r in s.Renderables) targetCanvas.Add(r);
        }

        internal Slice GetSlice(int index) => this.slices[index];
    }
}
