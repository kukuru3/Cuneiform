namespace Cuneiform {
    public abstract class UIBehaviour {
        public UIElement Element { get; private set; }
        protected abstract void Initialize();

        internal void AttachTo(UIElement uIElement) {
            Element = uIElement;
            Initialize();
        }

        internal void Detach() {
            Element = null;
        }
    }
}
