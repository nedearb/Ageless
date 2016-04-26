﻿
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ageless {

    public enum COMP_STATUS {
        NO_RENDER, READY_TO_MAKE, MAKING, READY_TO_COMPILE, READY_TO_RENDER
    }

    public abstract class Renderable {

        protected RenderMaker renderMaker;

        protected uint[] VBOIDs;
        protected int elementCount = 0;
        public COMP_STATUS compileState = COMP_STATUS.NO_RENDER;
        

		protected List<Vertex> vert = null;
		protected List<uint> ind = null;

		public bool markForRemoval = false;

        public Renderable(RenderMaker renderMaker) {
            this.renderMaker = renderMaker;
        }

        protected void cleanupRender() {
            compileState = COMP_STATUS.NO_RENDER;
            vert = null;
			ind = null;
			if (VBOIDs != null) {
				GL.DeleteBuffers(2, VBOIDs);
				VBOIDs = null;
			}
			markForRemoval = false;
        }

        abstract public void makeRender();

		public void addVert(ref Vector3 p, ref Vector3 normal, ref Vector2 UV, ref List<Vertex> vert, ref List<uint> ind, ref uint nextI) {
			Vertex v = new Vertex(p, UV, normal);
			vert.Add(v);
			ind.Add(nextI);
			nextI++;
		}

		public void addInd(ref uint nextI, int offset){
			ind.Add((uint)(nextI + offset));
		}

		//HORRIBLY SLOW:
		/*public void tryToAdd(ref Vector3 p, ref Vector3 normal, ref Vector2 UV, ref Dictionary<Vertex, uint> vert, ref List<uint> ind, ref uint nextI) {
			Vertex v = new Vertex(p, UV, normal);
			if (!vert.ContainsKey(v)) {
				vert.Add(v, nextI);
				nextI++;
			}
			ind.Add(vert[v]);
		}*/

		public void compileRender() {
			if(compileState != COMP_STATUS.READY_TO_COMPILE){
				Console.Error.WriteLine("ERROR!!");
			}

			//Console.WriteLine("(Render) Array creation");

			Vertex[] vertices = vert.ToArray();
            uint[] indecies = ind.ToArray();

            elementCount = ind.Count;

			Console.Out.WriteLine("(Render) Vert Count = " + vert.Count);
			Console.Out.WriteLine("(Render) Ind Count = " + ind.Count);


			//Console.WriteLine("(Render) Opengl Chunk compilation calls");

			if(VBOIDs == null) {
				VBOIDs = new uint[2];
				GL.GenBuffers(2, VBOIDs);
				Console.WriteLine("New Buffers: [{0}, {1}]", VBOIDs[0], VBOIDs[1]);
			}

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOIDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertex.StrideToEnd * vertices.Length), vertices, BufferUsageHint.StaticDraw);
			//GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOIDs[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * indecies.Length), indecies, BufferUsageHint.StaticDraw);
			//GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			//Console.WriteLine("(Render) done");

            vert = null;
            ind = null;

            compileState = COMP_STATUS.READY_TO_RENDER;

        }


        public void drawRender() {
            //Console.Out.WriteLine("Draw chunk: " + Location.X + ", " + Location.Y);

			if(markForRemoval){
				cleanupRender();
				return;
			}

            switch (compileState) {
                case COMP_STATUS.READY_TO_MAKE: {
                    renderMaker.list.Add(this);
                    compileState = COMP_STATUS.MAKING;
					return;
                }
                case COMP_STATUS.READY_TO_COMPILE: {
                    compileRender();
					drawRender();
					return;
                }
                case COMP_STATUS.READY_TO_RENDER: {

                    /*if (this as Chunk != null) {
						Console.Out.WriteLine("Draw chunk: {0}, {1}  VBO:[{2}, {3}] elCount={4}", (this as Chunk).Location.X, (this as Chunk).Location.Y, VBOIDs[0], VBOIDs[1], elementCount);
                    }else if (this as Actor != null) {
						Console.Out.WriteLine("Draw actor: {0}  VBO:[{1}, {2}] elCount={3}", (this as Actor).ID, VBOIDs[0], VBOIDs[1], elementCount);
					}*/

					GL.BindBuffer(BufferTarget.ArrayBuffer, VBOIDs[0]);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOIDs[1]);

                    GL.EnableVertexAttribArray(0); //Positions
                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.StrideToEnd, Vertex.StrideToPosition);

                    GL.EnableVertexAttribArray(1); //UV
                    GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.StrideToEnd, Vertex.StrideToUV);

                    GL.EnableVertexAttribArray(2); //Normals
                    GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vertex.StrideToEnd, Vertex.StrideToNormal);

					GL.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, (IntPtr)null);
					//GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
					//GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                    return;
                }
            }

        }

    }
}
