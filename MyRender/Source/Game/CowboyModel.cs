using grendgine_collada;
using MyRender.MyEngine;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MyRender.Game
{
    class CowboyModel : DaeModel
    {
        public override bool Loader(string path, bool loadAnimation = true)
        {
            var result = base.Loader(path, loadAnimation);
            if (!result) return result;

            Rotation(1, 0, 0, -90);


            foreach (var model in ModelList)
            {
                // generate render object
                Render render = Render.CreateRender(Resource.Instance.CreateCowboyM(), delegate (Render r)
                {
                    var m = r.MaterialData;

                    if (m.ShaderProgram != 0)
                    {
                        GL.UseProgram(m.ShaderProgram);

                        m.UniformTexture("TEX_COLOR", TextureUnit.Texture0, Material.TextureType.Color, 0);
                        m.UniformTexture("NORMAL_TEX_COLOR", TextureUnit.Texture1, Material.TextureType.Normal, 1);
                        var view_mat = GameDirect.Instance.MainScene.MainCamera.ViewMatrix;
                        m.UniformMatrix4("VIEW_MAT", ref view_mat, true);

                        var joints = Animation.HashJoint[0];
                        for (int i = 0; i < joints.Length; i++)
                        {
                            m.UniformMatrix4("jointTransforms[" + i.ToString() + "]", ref joints[i].animatedTransform, true);
                        }

                    }
                },
                this,
                model,
                Render.Normal);
                render.AddVertexAttribute("tangent", model.TangentBuffer, 3, false);
                render.AddVertexAttribute("jointIndices", model.JointBuffer, 4, false);
                render.AddVertexAttribute("weight", model.WeightBuffer, 4, false);

                RenderList.Add(render);

            }

            Animation.animator.DoAnimation(Animation.AnimationData);

            return true;

        }

        protected override void skeletonLoader(Grendgine_Collada_Library_Visual_Scenes l_s, MeshSkinData[] meshSkin)
        {
            base.skeletonLoader(l_s, meshSkin);

            var jointNode = l_s.Visual_Scene[0].Node[1].node[0];
            var result = loadJointData(jointNode);

            //result.CalcInverseBindTransform(Matrix4.Identity);
            Animation.JointHierarchy.Add(result);

            // create joint table
            foreach (var mesh in meshSkin)
            {
                Animation.CreateHashJoint(mesh.Joints, mesh.InversBind);
            }
        }

        public override void OnStart()
        {
            base.OnStart();

        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

            if (Animation != null)
            {
                Animation.animator.Update((float)e.Time);
            }

        }

        //public override void OnRender(FrameEventArgs e)
        //{
        //    base.OnRender(e);

        //    foreach (var model in ModelList)
        //    {

        //        if (SetUpShaderAction.ContainsKey(model.id)) SetUpShaderAction[model.id]();

        //        // bind vertex buffer 
        //        GL.BindBuffer(BufferTarget.ArrayBuffer, model.VBO);
        //        GL.EnableClientState(ArrayCap.VertexArray);
        //        GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

        //        // bind normal buffer
        //        GL.BindBuffer(BufferTarget.ArrayBuffer, model.NBO);
        //        GL.EnableClientState(ArrayCap.NormalArray);
        //        GL.NormalPointer(NormalPointerType.Float, 0, 0);

        //        // bind texture coord buffer
        //        GL.BindBuffer(BufferTarget.ArrayBuffer, model.TBO);
        //        GL.EnableClientState(ArrayCap.TextureCoordArray);
        //        GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);

        //        // tangent buffer
        //        var tangent = GL.GetAttribLocation(MaterialData.ShaderProgram, "tangent");
        //        GL.EnableVertexAttribArray(tangent);
        //        GL.BindBuffer(BufferTarget.ArrayBuffer, model.TangentBuffer);
        //        GL.VertexAttribPointer(tangent, 3, VertexAttribPointerType.Float, false, 0, 0);

        //        // joint indices buffer
        //        var jointIndices = GL.GetAttribLocation(MaterialData.ShaderProgram, "jointIndices");
        //        GL.EnableVertexAttribArray(jointIndices);
        //        GL.BindBuffer(BufferTarget.ArrayBuffer, model.JointBuffer);
        //        GL.VertexAttribPointer(jointIndices, 4, VertexAttribPointerType.Float, false, 0, 0);

        //        // weight buffer
        //        var weight = GL.GetAttribLocation(MaterialData.ShaderProgram, "weight");
        //        GL.EnableVertexAttribArray(weight);
        //        GL.BindBuffer(BufferTarget.ArrayBuffer, model.WeightBuffer);
        //        GL.VertexAttribPointer(weight, 4, VertexAttribPointerType.Float, false, 0, 0);

        //        //GL.BindTexture(TextureTarget.Texture2D, MaterialData.TextureID);
        //        GameDirect.Instance.DrawCall(model.DrawType, model.Vertices.Length);

        //        GL.DisableClientState(ArrayCap.VertexArray);
        //        GL.DisableClientState(ArrayCap.NormalArray);
        //        GL.DisableClientState(ArrayCap.TextureCoordArray);

        //        GL.DisableVertexAttribArray(tangent);
        //        GL.DisableVertexAttribArray(jointIndices);
        //        GL.DisableVertexAttribArray(weight);


        //        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        //        GL.BindTexture(TextureTarget.Texture2D, 0);
        //    }

        //}

    }
}
