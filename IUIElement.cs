using Sargon.Graphics;

using Ur.Geometry;

namespace Cuneiform {
    public interface IUIElement {
        UISystem UI { get; }
        public Rect LocalRect { get; }
        public Rect WorldRect { get; }
        public Rect DrawRect { get; } // total rect as opposed to DRAW ROOT

        Style EffectiveStyle { get; }
    }

    public interface IHasColor {
        Ur.Color? Color { get; set; }
    }

    public interface IHasRenderTexture {
        TextureCanvas Canvas { get; }
        bool ToTexture { get; }
    }
}
