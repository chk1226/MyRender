using OpenTK;

namespace MyRender.MyEngine
{
    class Scene : Node
    {
        private Camera _mainCamera;
        public Camera MainCamera
        {
            get
            {
                return _mainCamera;
            }
            private set
            {
                _mainCamera = value;
            }
        }

        public Scene()
        {
            MainCamera =  new Camera(new Vector3(0, 0, 10),
                                new Vector3(0, 0, 0),
                                new Vector3(0, 1, 0),
                                45,
                                1,
                                1000,
                                MainWindow.Instance.ClientRectangle);

            MainCamera.Apply();
        }



    }
}
