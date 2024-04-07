/*
 Ehab Mahmoud Ali Mahmoud Fahmy
 20201700177
 CS
 Section 2
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

//include GLM library
using GlmNet;


using System.IO;
using System.Diagnostics;

namespace Graphics
{
    class Renderer
    {
        Shader sh;
        
        uint cuboidBufferID;
        //uint circleBufferID;
        uint cylinderBufferID;
        uint xyzAxesBufferID;
        uint wallFloorBufferID;

        //3D Drawing
        mat4 ModelMatrix;
        mat4 ModelMatrix2;
        mat4 ViewMatrix;
        mat4 ProjectionMatrix;
        
        int ShaderModelMatrixID;
        int ShaderViewMatrixID;
        int ShaderProjectionMatrixID;

        const int CIRCLE_EDGES = 50;
        const float rotationSpeed = 1f;
        float rotationAngle = 0;

        public float translationX=0, 
                     translationY=0, 
                     translationZ=0;
        vec3 scale = new vec3(1, 1, 1);

        Stopwatch timer = Stopwatch.StartNew();

        Texture floor_texture;
        Texture box_texture;
        Texture clock_texture;

        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            Gl.glClearColor(0, 0, 0.4f, 1);

            floor_texture = new Texture(projectPath + "\\Textures\\texture1.jpg", 1);
            box_texture = new Texture(projectPath + "\\Textures\\crate.jpg", 2);
            clock_texture = new Texture(projectPath + "\\Textures\\clock.jpg", 3);

            float[] cuboidVerts = {
                //Bottom Base
		        0.0f,  0.0f, 0.0f, 0.0f, 0.0f, 0.0f,     0,0,
                10.0f,  0.0f, 0.0f, 0.0f, 0.0f, 0.0f,    1,0,
                10.0f,  40.0f, 0.0f, 0.0f, 0.0f, 0.0f,   1,1,
                0.0f,  40.0f, 0.0f, 0.0f, 0.0f, 0.0f,    0,1,



                0.0f,  0.0f, 0.0f, 0.0f, 0.0f, 0.0f,              0,0,
                0.0f,  0.0f, 20.0f, 0.0f, 0.0f, 0.0f,             1,0,
                10.0f,  0.0f, 20.0f, 0.0f, 0.0f, 0.0f,            1,1,
                10.0f,  0.0f, 0.0f, 0.0f, 0.0f, 0.0f,             0,1,

                0.0f,  0.0f, 0.0f, 0.0f, 0.0f, 0.0f,                 0,0,
                0.0f,  0.0f, 20.0f, 0.0f, 0.0f, 0.0f,                1,0,
                0.0f,  40.0f, 20.0f, 0.0f, 0.0f, 0.0f,               1,1,
                0.0f,  40.0f, 0.0f, 0.0f, 0.0f, 0.0f,                0,1,

                0.0f,  40.0f, 20.0f, 0.0f, 0.0f, 0.0f,               0,0,
                0.0f,  40.0f, 0.0f, 0.0f, 0.0f, 0.0f,                1,0,
                10.0f,  40.0f, 0.0f, 0.0f, 0.0f, 0.0f,               1,1,
                10.0f,  40.0f, 20.0f, 0.0f, 0.0f, 0.0f,              0,1,

                10.0f,  40.0f, 0.0f, 0.0f, 0.0f, 0.0f,              0,0,
                10.0f,  40.0f, 20.0f, 0.0f, 0.0f, 0.0f,             1,0,
                10.0f,  0.0f, 20.0f, 0.0f, 0.0f, 0.0f,              1,1,
                10.0f,  0.0f, 0.0f, 0.0f, 0.0f, 0.0f,               0,1,

                //Top Base
                0.0f,  0.0f, 20.0f, 0.0f, 0.0f, 0.0f,               0,0,
                10.0f,  0.0f, 20.0f, 0.0f, 0.0f, 0.0f,              1,0,
                10.0f,  40.0f, 20.0f, 0.0f, 0.0f, 0.0f,             1,1,
                0.0f,  40.0f, 20.0f, 0.0f, 0.0f, 0.0f,              0,1
            };


            float[] xyzAxesVertices = {
		        //x
		        0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, //R
		        100.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, //R
		        //y
	            0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, //G
		        0.0f, 100.0f, 0.0f, 0.0f, 1.0f, 0.0f, //G
		        //z
	            0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,  //B
		        0.0f, 0.0f, 100.0f, 0.0f, 0.0f, 1.0f,  //B
            };

            float[] circleVerts = draw_circle(0, 30, 30, 10, 0, 0, 0);
            var cylinderPoints = draw_cylinder_side();

            float[] wallFloorVerts = {
                //floor
                0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,   0,0,
                100.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,  4,0,
                100.0f, 100.0f, 0.0f, 0.0f, 0.0f, 0.0f,  4,4,
                0.0f, 100.0f, 0.0f, 0.0f, 0.0f, 0.0f,    0,4,

                //wall
		        0.0f, 0.0f, 0.0f, 0.6f, 0.4f, 0.7f,   0,0,
                0.0f, 100.0f, 0.0f, 0.6f, 0.4f, 0.7f,   0,0,
                0.0f, 100.0f, 100.0f, 0.6f, 0.4f, 0.7f,   0,0,
                0.0f, 0.0f, 100.0f, 0.6f, 0.4f, 0.7f,   0,0
            };


            cuboidBufferID = GPU.GenerateBuffer(cuboidVerts);
            //circleBufferID = GPU.GenerateBuffer(circleVerts);
            cylinderBufferID = GPU.GenerateBuffer(cylinderPoints);
            xyzAxesBufferID = GPU.GenerateBuffer(xyzAxesVertices);
            wallFloorBufferID = GPU.GenerateBuffer(wallFloorVerts);

            // View matrix 
            ViewMatrix = glm.lookAt(
                        new vec3(50, 50, 50), // Camera is at (0,5,5), in World Space
                        new vec3(0, 0, 0), // and looks at the origin
                        new vec3(0, 0, 1)  // Head is up (set to 0,-1,0 to look upside-down)
                );
            // Model Matrix Initialization
            ModelMatrix = new mat4(1);
            ModelMatrix2 = new mat4(1);

            //ProjectionMatrix = glm.perspective(FOV, Width / Height, Near, Far);
            ProjectionMatrix = glm.perspective(45.0f, 4.0f / 3.0f, 0.1f, 100.0f);
            
            // Our MVP matrix which is a multiplication of our 3 matrices 
            sh.UseShader();


            //Get a handle for our "MVP" uniform (the holder we created in the vertex shader)
            ShaderModelMatrixID = Gl.glGetUniformLocation(sh.ID, "modelMatrix");
            ShaderViewMatrixID = Gl.glGetUniformLocation(sh.ID, "viewMatrix");
            ShaderProjectionMatrixID = Gl.glGetUniformLocation(sh.ID, "projectionMatrix");

            Gl.glUniformMatrix4fv(ShaderViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());

            timer.Start();
        }

        public void Draw()
        {
            sh.UseShader();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

            #region XYZ axis
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, xyzAxesBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, new mat4(1).to_array()); // Identity

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
             
            Gl.glDrawArrays(Gl.GL_LINES, 0, 6);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            #endregion


            #region WallFloor
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, wallFloorBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            floor_texture.Bind();
            Gl.glDrawArrays(Gl.GL_QUADS, 0, 8);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            Gl.glDisableVertexAttribArray(2);
            #endregion

            #region Cuboid
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, cuboidBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            box_texture.Bind();
            Gl.glDrawArrays(Gl.GL_QUADS, 0, 6 * 4);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            Gl.glDisableVertexAttribArray(2);
            #endregion


        }


        public float[] draw_circle(float centerX, float centerY, float centerZ, float radius, float R, float G, float B)
        {
            List<float> verticies = new List<float>();

            float step = (float)(2 * Math.PI) / CIRCLE_EDGES;

            float angle = 0.0f;
            while (angle < 2 * Math.PI)
            {
                float u = 0.5f + (0.5f * (float)Math.Cos(angle));
                float v = 0.5f - (0.5f * (float)Math.Sin(angle));
                float x = centerX + (float)(radius * Math.Cos(angle));
                float y = centerY + (float)(radius * Math.Sin(angle));
                verticies.AddRange(new float[] { x, y, centerZ, R, G, B, u, v });
                angle += step;
            }

            return verticies.ToArray();
        }

        public float[] draw_cylinder_side()
        {
            float centerX = 30, centerY = 30, centerZ = 0, radius = 10, height = 30;
            float R = 0.6f, G = 0.2f, B = 0.4f;
            List<float> verticies = new List<float>();

            float step = (float)(2 * Math.PI) / CIRCLE_EDGES;

            float angle = 0.0f;
            while (angle < 2 * Math.PI)
            {
                float x = centerX + (float)(radius * Math.Cos(angle));
                float y = centerY + (float)(radius * Math.Sin(angle));
                verticies.AddRange(new float[] { x, y, centerZ, R, G ,B });
                verticies.AddRange(new float[] { x, y, centerZ + height, R, G, B });
                angle += step;
            }
            verticies.AddRange(verticies.GetRange(0, 12));

            return verticies.ToArray();
        }

        public void Update()
        {
            timer.Stop();
            var deltaTime = timer.ElapsedMilliseconds / 1000.0f;

            vec3 circleCenter = new vec3(0, 30, 30);
            List<mat4> cylinderTransformations = new List<mat4>
            {
                glm.translate(new mat4(1), -1 * circleCenter),
                glm.rotate((float) (Math.PI / 2), new vec3(0, 1, 0)),
                glm.rotate((float) (Math.PI / 2), new vec3(1, 0, 0)),
                glm.translate(new mat4(1), circleCenter),
            };
            ModelMatrix2 = MathHelper.MultiplyMatrices(cylinderTransformations);

            timer.Reset();
            timer.Start();
        }

        public void OnKeyboardKeyPress(char key)
        {
            float speed = 5;

            if (key == 'd')
                translationX += speed;
            if (key == 'a')
                translationX -= speed;

            if (key == 'w')
                translationY += speed;
            if (key == 's')
                translationY -= speed;

            if (key == 'z')
                translationZ += speed;
            if (key == 'c')
                translationZ -= speed;

            if (key == 't')
            {
                scale.x *= 1.2f;
                scale.y *= 1.2f;
                scale.z *= 1.2f;
            }

            if (key == 'y')
            {
                scale.x /= 1.2f;
                scale.y /= 1.2f;
                scale.z /= 1.2f;
            }
        }

        public void CleanUp()
        {
            sh.DestroyShader();
        }


    }
}
