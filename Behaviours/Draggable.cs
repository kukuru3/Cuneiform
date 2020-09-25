namespace Cuneiform.Behaviours {
    public class Draggable : UIBehaviour {
        protected override void Initialize() {
            Element.UI.UIFrame += OnUIFrame;
        }

        bool dragging = true;
        private void OnUIFrame() { 
            
            var keyState = Element.UI.Context.Input.GetKey(Sargon.Input.Keys.Mouse1);
            bool overUs = Element.UI.Highlighted == Element;
            if (keyState.IsPressed) {
                if (overUs) dragging = true;
            } else if (keyState.IsHeld) {
                if (dragging) {
                    var d = Element.UI.MouseDelta;
                    Element.LocalRect = Element.LocalRect.Move(d.X, d.Y);
                }
            } else if (keyState.IsRaised) {
                dragging = false;
            }
        }
    }
}
