using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace MyRender.MyEngine
{
    public enum ProjectType
    {
        Perspective,
        Orthographic
    }

    //public struct Rect
    //{
    //    public float x;
    //    public float y;
    //    public float Width;
    //    public float Height;

    //    public Rect(float x, float y, float w, float h)
    //    {
    //        this.x = x;
    //        this.y = y;
    //        this.Width = w;
    //        this.Height = h;
    //    }
    //}

    class Camera
    {
        public Camera(Vector3 eye, Vector3 focus, Vector3 vUp, float fovy, float zNear, float zFar, Rectangle viewport)
        {
            this.Viewport = viewport;

            // Calculate aspect ratio, checking for divide by zero
            float aspect = 1.0f;
            if (this.Viewport.Height > 0)
            {
                aspect = this.Viewport.Width / this.Viewport.Height;
            }

            // Setup a perspective view
            ProjectMatix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fovy), aspect, zNear, zFar);

            this.ViewMatrix = LookAt(ref eye, ref focus, ref vUp);
        }

        public ProjectType ProjectMode { get; set; }

        private Rectangle _viewport;
        public Rectangle Viewport
        {
            get { return _viewport; }
            set {
                _viewport = value;
                GL.Viewport(_viewport);
            }
        }

        private Matrix4 _projectMatrix;
        public Matrix4 ProjectMatix
        {
            get { return _projectMatrix;}
            set {
                _projectMatrix = value;
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.LoadMatrix(ref this._projectMatrix);
            }
        }
        public Matrix4 ViewMatrix { get; set; }

        public static Matrix4 LookAt(ref Vector3 eye, ref Vector3 focus, ref Vector3 vup)
        {
            Vector3 f = focus - eye;
            f.Normalize();

            vup.Normalize();

            Vector3 s = Vector3.Cross(f, vup);
            s.Normalize();

            vup = Vector3.Cross(s, f);
            vup.Normalize();

            Matrix4 m = new Matrix4(s.X, s.Y, s.Z, -Vector3.Dot(s, eye),
                                    vup.X, vup.Y, vup.Z, -Vector3.Dot(vup, eye),
                                    -f.X, -f.Y, -f.Z, Vector3.Dot(f, eye),
                                    0.0f, 0.0f, 0.0f, 1.0f);
            m.Transpose();
            return m;
        }


    }
}
