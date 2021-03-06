﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ageless {
	public abstract class Actor {

		public static readonly float GRAVITY = 0.1f;

		public uint ID;

        protected float movementSpeed = 1.0f / 4.0f;
        public float maxHeightChange = 2.0f;
        protected float radius = 1.0f;

		public Vector3 position;
		public Vector3 rotation;

        public Skeleton skeleton;


        public Actor() {

		}

		public abstract void update(Game game);

        public void draw(Game game) {
            Matrix4 pos = Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationZ(rotation.Z) * Matrix4.CreateTranslation(position);
            skeleton.draw(game, pos);
		}
    }
}
