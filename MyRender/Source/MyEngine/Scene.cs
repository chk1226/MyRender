﻿using OpenTK;
using System;

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

        public WeakReference<Light> SceneLight;

        public Scene()
        {
            MainCamera =  new Camera(new Vector3(45, 45, 12),
                                new Vector3(0, 2, 0),
                                new Vector3(0, 1, 0),
                                45,
                                1,
                                1000,
                                MainWindow.Instance.ClientRectangle);

            MainCamera.Apply();
        }



    }
}
