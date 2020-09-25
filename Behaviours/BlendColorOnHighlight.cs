using Ur;

namespace Cuneiform.Behaviours {
    public class BlendColorOnHighlight : UIBehaviour {

        Color? baseColor;
        Color? highlightColor;
        IUIElement clickableOwner;

        public BlendColorOnHighlight(Color highlightColor) {
            this.highlightColor = highlightColor;

            if (Element is IHasColor withColor)
                baseColor = withColor.Color;
        }

        protected override void Initialize() {
            for (var e = Element; e != null; e = Element.Parent) {
                if (e.Clickable) {
                    clickableOwner = e;
                    break;
                }
            }
            Element.UI.HighlightChanged -= UpdateColor;
            Element.UI.HighlightChanged += UpdateColor;
        }

        private void UpdateColor() {
            ((IHasColor)Element).Color = (Element.UI.Highlighted == clickableOwner) ? highlightColor : baseColor;
        }
    }
}
