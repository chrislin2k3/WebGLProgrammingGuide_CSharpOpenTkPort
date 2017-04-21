﻿// This is an example from the book "WebGL Programming Guide"
// by Kouichi Matsuda and Rodger Lea

// Author of this port to OpenTK
// Full Name: Ivan Enzhaev
// Nick Name: 8Observer8
// Email: 8observer8@gmail.com

using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utils;

namespace _01_RotatedTriangle_Matrix4
{
    class MainWindow : GameWindow
    {
        private bool canDraw = false;
        private int program;
        private int nVertices;
        private int u_xformMatrix;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Title = "Rotate A Triangle (Matrix4)";
            Width = 400;
            Height = 400;

            Console.WriteLine("Version: " + GL.GetString(StringName.Version));
            Console.WriteLine("Video Adapter: " + GL.GetString(StringName.Renderer));

            // Load shaders from files
            string vShaderSource = null;
            string fShaderSource = null;
            ShaderLoader.LoadShader("./Shaders/VertexShader.glsl", out vShaderSource);
            ShaderLoader.LoadShader("./Shaders/FragmentShader.glsl", out fShaderSource);
            if (vShaderSource == null)
            {
                Logger.Append("Failed to load the vertex shader from a file");
                return;
            }
            if (fShaderSource == null)
            {
                Logger.Append("Failed to load the fragment shader from a file");
                return;
            }

            // Initialize shaders
            if (!ShaderLoader.InitShaders(vShaderSource, fShaderSource, out program))
            {
                Logger.Append("Failed to initialize the shaders");
                return;
            }

            // Write the positions of vertices to a vertex shader
            nVertices = InitVertexBuffers();
            if (nVertices < 0)
            {
                Logger.Append("Failed to write the positions of vertices to a vertex shader");
                return;
            }

            // Create Matrix4 object for the rotation matrix
            Matrix4 xformMatrix = new Matrix4();

            // Set the rotation matrix
            float ANGLE = 90 * (float)Math.PI / 180f;
            xformMatrix = Matrix4.CreateRotationZ(ANGLE);

            // Pass the rotation matrix to the vertex shader
            u_xformMatrix = GL.GetUniformLocation(program, "u_xformMatrix");
            if (u_xformMatrix < 0)
            {
                Logger.Append("Failed to get the storage location of u_xformMatrix");
                return;
            }
            GL.UniformMatrix4(u_xformMatrix, false, ref xformMatrix);

            // Specify the color for clearing the canvas
            GL.ClearColor(Color.Black);

            canDraw = true;
        }

        private int InitVertexBuffers()
        {
            float[] vertices = new float[]
            {
                0f, 0.5f, -0.5f, -0.5f, 0.5f, -0.5f
            };
            int n = 3; // The number of vertices

            // Create a buffer object
            int vertexBuffer;
            GL.GenBuffers(1, out vertexBuffer);
            if (vertexBuffer < 0)
            {
                Logger.Append("Failed to create the buffer object");
                return -1;
            }

            // Bind the buffer object to target
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            // Write data into the buffer object
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Get the storage location of a_Position
            int a_Position = GL.GetAttribLocation(program, "a_Position");
            if (a_Position < 0)
            {
                Logger.Append("Failed to get the storage location of a_Position");
                return -1;
            }

            // Assign the buffer object to a_Position variable
            GL.VertexAttribPointer(a_Position, 2, VertexAttribPointerType.Float, false, 0, 0);

            // Enable the assignment to a_Position variable
            GL.EnableVertexAttribArray(a_Position);

            return n;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnRenderFrame(e);

            if (canDraw)
            {
                // Draw the triangle
                GL.DrawArrays(PrimitiveType.Triangles, 0, nVertices);
            }

            GL.Flush();
            SwapBuffers();
        }
    }
}