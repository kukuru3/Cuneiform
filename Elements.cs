using Sargon.Graphics;

namespace Cuneiform {
    public class Group : UIElement {
        public override string Print => "Group";
    }

    public class Panel : UIElement, IHasRenderTexture {
        public bool ToTexture { get; }

        public Panel(bool toTexture = false) {
            ToTexture = toTexture;
            if (toTexture) Canvas = new TextureCanvas((0,0,1,1));
        }

        public override string Print => $"Panel {LocalRect.W}x{LocalRect.H}";

        public TextureCanvas Canvas { get; private set; }

        protected override void OnLocalRectUpdated() {

        }

        internal override void Update() {
            base.Update();
            if (!ToTexture) return;
            if (tree != null)
                Canvas.Rect = WorldRect;
        }
    }

    public class Button : UIElement {

        public Button() {
            Clickable = true;
        }
    }

    public class Label : UIElement, IHasColor {

        public string Text { get; set; }

        public Text.Anchors TextAnchor { get; set; }
        public Ur.Color? Color { get; set; }

        public override string Print => $"Label '{Text}'";

        Slices.Text textSlice;

        void CreateTextSlice() {
            textSlice = new Slices.Text();
            AddSlice(textSlice);
        }

        internal override void Update() {
            if (textSlice == null) CreateTextSlice();
            base.Update();
            var s = EffectiveStyle;
            var textObject = textSlice.SourceText;

            textObject.Content = Text;
            var f = s?.Font;
            if (f != null && f != textObject.Font) textObject.Font = f;
            textObject.Anchor = TextAnchor;
            if (s?.FontSize > 0) textObject.CharacterSize = s.FontSize;
            textObject.Rect = DrawRect;
            textObject.Color = Color ?? s?.FontColor ?? Ur.Color.White;
        }

    }
}
