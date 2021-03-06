﻿using MyRender.Debug;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace MyRender.MyEngine
{
    public enum ProjectType
    {
        Perspective,
        Orthographic
    }

    class Camera : Node
    {
        // Perspective camera
        public Camera(Vector3 eye_rotation, Vector3 focus, Vector3 vUp, float fovy, float zNear, float zFar, Rectangle viewport)
        {
            this._viewport = viewport;

            // Calculate aspect ratio, checking for divide by zero
            float aspect = 1.0f;
            if (this._viewport.Height > 0)
            {
                aspect = (float)this._viewport.Width / this._viewport.Height;
            }

            // Setup a perspective view
            this.fovy = fovy;
            this.zNear = zNear;
            this.zFar = zFar;
            _projectMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fovy), aspect, zNear, zFar);

            _projectMatrix.Transpose();

            // Setup a view matrix
            this.eye_rotation = eye_rotation;
            this.eye = eyePosCalculate(focus, eye_rotation);
            this.focus = focus;
            this.vUp = vUp;
            _viewMatrix = Matrix4.LookAt(eye, focus, vUp);
            _viewMatrix.Transpose();
            
            ProjectMode = ProjectType.Perspective;

            // Setup ui view and project matrix
            UIViewMatrix = Matrix4.LookAt(Vector3.UnitZ, Vector3.Zero, Vector3.UnitY);
            UIViewMatrix.Transpose();
            UIProjectMatrix = Matrix4.CreateOrthographicOffCenter(0, _viewport.Width, _viewport.Height, 0, 0.125f, 1.125f);
            UIProjectMatrix.Transpose();
        }

        public ProjectType ProjectMode { get; set; }

        private Rectangle _viewport;
        public Rectangle Viewport
        {
            get { return _viewport; }
            private set {
                _viewport = value;
                GL.Viewport(_viewport);
            }
        }

        public float fovy { get; private set; }
        public float zNear { get; private set; }
        public float zFar { get; private set; }
        public Vector3 eye { get; private set; }
        public Vector3 focus { get; private set; }
        public Vector3 vUp { get; private set; }
        private Vector3 eye_rotation;
        public Vector3 EyeRotation { get { return eye_rotation; } }

        private Matrix4 _projectMatrix;
        public Matrix4 ProjectMatix
        {
            get { return _projectMatrix;}
            set {
                _projectMatrix = value;
                GL.MatrixMode(MatrixMode.Projection);
                var t = Matrix4.Transpose(this._projectMatrix);
                GL.LoadMatrix(ref t);
            }
        }
        
        private Matrix4 _viewMatrix;
        public Matrix4 ViewMatrix
        {
            get { return this._viewMatrix; }
            set {
                _viewMatrix = value;
                GL.MatrixMode(MatrixMode.Modelview);
                var t = Matrix4.Transpose(this._viewMatrix);
                GL.LoadMatrix(ref t);
            }
        }

        public Matrix4 UIViewMatrix;
        public Matrix4 UIProjectMatrix;


        /// <summary>
        /// Apply view matrix, project, viewport
        /// </summary>
        public void Apply()
        {
            // view matrix
            ViewMatrix = _viewMatrix;

            // project
            ProjectMatix = _projectMatrix;

            // view port
            Viewport = _viewport;
        }

        public void UpdateViewport(Rectangle vp)
        {
            Viewport = vp;
            float aspect = 1.0f;
            if (vp.Height > 0)
            {
                aspect = (float)vp.Width / vp.Height;
            }

            //Setup a perspective view
            ProjectMatix = Matrix4.Transpose(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fovy), aspect, zNear, zFar));

            //Setup ui project
            UIProjectMatrix = Matrix4.CreateOrthographicOffCenter(0, vp.Width, vp.Height, 0, 0.125f, 1.125f);
            UIProjectMatrix.Transpose();

        }

        private void applyRotation()
        {
            if (eye_rotation.Y <= 5.0f) eye_rotation.Y = 5.0f;
            else if (eye_rotation.Y >= 175.0f) eye_rotation.Y = 175.0f;

            eye = eyePosCalculate(focus, eye_rotation);
            ViewMatrix = Matrix4.Transpose(Matrix4.LookAt(eye, focus, vUp));
        }

        public void RotationScreen(float a_x, float a_y)
        {
            
            eye_rotation.X += a_x;
            eye_rotation.Y += a_y;

            applyRotation();

            //Log.Print(eye_rotation.ToString());
        }

        public void ResetRotation(float x, float y)
        {
            eye_rotation.X = x;
            eye_rotation.Y = y;

            applyRotation();
        }

        private void applyZoomInOut(float min, float max)
        {
            if (eye_rotation.Z >= max) eye_rotation.Z = max;
            if (eye_rotation.Z <= min) eye_rotation.Z = min;

            eye = eyePosCalculate(focus, eye_rotation);
            ViewMatrix = Matrix4.Transpose(Matrix4.LookAt(eye, focus, vUp));
        }

        public void ZoomInOut(float delta_z, float min, float max)
        {
            eye_rotation.Z += delta_z;
            applyZoomInOut(min, max);

            Log.Print(eye_rotation.Z.ToString());
        }

        public void ResetZoomInOut(float z, float min, float max)
        {
            eye_rotation.Z = z;
            applyZoomInOut(min, max);

        }


        // 1 parameter: focus, 2 parameter: eye_rotation
        private Vector3 eyePosCalculate(Vector3 f, Vector3 p)
        {
            return new Vector3(f.X + p.Z * (float)Math.Sin(MathHelper.DegreesToRadians(p.Y)) * (float)Math.Cos(MathHelper.DegreesToRadians(p.X)),
                               f.Y + p.Z * (float)Math.Cos(MathHelper.DegreesToRadians(p.Y)),
                               f.Z + p.Z * (float)Math.Sin(MathHelper.DegreesToRadians(p.Y)) * (float)Math.Sin(MathHelper.DegreesToRadians(p.X)));
        }


        //public static Matrix4 MyLookAt(Vector3 eye, Vector3 focus, Vector3 vup)
        //{
        //    Vector3 f = focus - eye;
        //    f.Normalize();

        //    vup.Normalize();

        //    Vector3 s = Vector3.Cross(f, vup);
        //    s.Normalize();

        //    vup = Vector3.Cross(s, f);
        //    vup.Normalize();

        //    Matrix4 m = new Matrix4(s.X, s.Y, s.Z, -Vector3.Dot(s, eye),
        //                            vup.X, vup.Y, vup.Z, -Vector3.Dot(vup, eye),
        //                            -f.X, -f.Y, -f.Z, Vector3.Dot(f, eye),
        //                            0.0f, 0.0f, 0.0f, 1.0f);
        //    return m;
        //}


    }
}
