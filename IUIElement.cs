using Ur.Geometry;

namespace Cuneiform {
    public interface IUIElement {
        UISystem UI { get; }
        public Rect LocalRect { get; }
        public Rect WorldRect { get; }

        Style EffectiveStyle { get; }
    }
}
