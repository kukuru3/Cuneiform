using System.Collections.Generic;
using Sargon.Assets;
using Sargon.Graphics;
using Ur;

namespace Cuneiform {
    /// <summary> A graphical layer of an UI element. Will usually result in one or more SFML drawables</summary>
    public abstract class Slice {
        protected IRenderable sourceRenderable;

        public string Name { get; set; }

        internal IUIElement Element { get; private set; }

        internal IEnumerable<IRenderable> Renderables => new[] { sourceRenderable };

        internal void Init(IUIElement element) {
            Element = element;
            Initialize();
        }

        protected abstract void Initialize();

        internal virtual void Cleanup() { 
            foreach (var renderable in Renderables) renderable?.OnCanvas?.Remove(renderable);
        }

        internal virtual void Update() { 

        }
    }
}

namespace Cuneiform.Slices {
    /// <summary>Creates a ninegrid, but needs to have an appropriate texture supplied</summary>
    public class BoxShadow : Slice {
        public int TextureMargin { get; set; } = 32;
        public float RenderMargin { get; set; } = 32f;
        public Ur.Geometry.Vector2 Offset { get; set; }

        public SpriteDefinition Source { get; set; }

        public BoxShadow() {
            Color = Color.Black;
        }

        public Color Color { get; set; }

        protected override void Initialize() {
            sourceRenderable = new Ninegrid {
                Source = Source,
                RenderedMargin = RenderMargin,
                TextureMargin = TextureMargin,
                Color = Color
            };
        }

        internal override void Update() {
            var shadow = (Ninegrid)sourceRenderable;
            shadow.SetRect(Element.DrawRect.Move(Offset.x, Offset.y).Expand(RenderMargin));
        }
    }

    /// <summary> A simple slice that renders an image without any processing.</summary>
    public class Image : Slice {

        private SpriteDefinition source;
        public SpriteDefinition Source { 
            get => source;
            set {
                if (source == value) return;
                source = value;
                if (sourceRenderable is Sprite s) s.Source = source;
            }
        }

        public Color Color { get; set; }

        public Ur.Grid.Rect? Subrect { get; set; }

        public Image() {
            Color = Color.White;
        }

        protected override void Initialize() {
            var spr = new Sprite();
            spr.Source = this.Source;
            sourceRenderable = spr;
        }

        internal override void Update() {
            var sr =(Sprite)sourceRenderable;
            sr.Fit(Element.DrawRect);
            sr.Color = Color;
            if (Subrect.HasValue) sr.OverrideTextureSubrect(Subrect.Value);
        }
    }

    public class Text : Slice {
        public string Content { get; set; }
        internal Sargon.Graphics.Text SourceText => (Sargon.Graphics.Text)sourceRenderable;

        protected override void Initialize() {
            sourceRenderable = new Sargon.Graphics.Text("");
            SourceText.Content = Content;
        }
    }
}
