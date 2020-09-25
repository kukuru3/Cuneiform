using Sargon.Graphics;

namespace Cuneiform {
    public class Group : UIElement {

        public override string Print => "Group";
        protected override void CreateSlices() { }

    }

    public class Panel : UIElement {

        public override string Print => $"Panel {LocalRect.W}x{LocalRect.H}";

        protected override void CreateSlices() { }

    }

    public class Button : UIElement {

        public Button() {
            Clickable = true;
        }

        protected override void CreateSlices() { }
    }

    public class Label : UIElement, IHasColor {

        public string Text { get; set; }

        public Text.Anchors TextAnchor { get; set; }
        public Ur.Color? Color { get; set; }

        public override string Print => $"Label '{Text}'";

        Slices.Text textSlice;

        protected override void CreateSlices() {
            textSlice = new Slices.Text();
            AddSlice(textSlice);
        }

        internal override void Update() {
            base.Update();
            var s = EffectiveStyle;
            var textObject = textSlice.SourceText;

            textObject.Content = Text;
            var f = s?.Font;
            if (f != null && f != textObject.Font) textObject.Font = f;
            textObject.Anchor = TextAnchor;
            if (s?.FontSize > 0) textObject.CharacterSize = s.FontSize;
            textObject.Rect = WorldRect;
            textObject.Color = Color ?? s?.FontColor ?? Ur.Color.White;
        }

    }
}
