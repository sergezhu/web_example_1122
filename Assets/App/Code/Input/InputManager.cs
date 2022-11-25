namespace App.Code.Input
{

	public class InputManager
    {
        private Controls _controls;


        public void Initialize()
        {
            _controls = new Controls();
            MainActions = _controls.Main;
			MainActions.Enable();
        }


        #region IInputManager

        public Controls.MainActions MainActions { get; private set; }

        #endregion
    }
}