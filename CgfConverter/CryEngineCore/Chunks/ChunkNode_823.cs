﻿using Extensions;
using System;
using System.IO;
using System.Numerics;

namespace CgfConverter.CryEngineCore
{
    public class ChunkNode_823 : ChunkNode {
        public override void Read(BinaryReader b)
        {
            base.Read(b);

            Name = b.ReadFString(64);
            if (string.IsNullOrEmpty(Name))
                Name = "unknown";

            ObjectNodeID = b.ReadInt32(); // Object reference ID
            ParentNodeID = b.ReadInt32();
            __NumChildren = b.ReadInt32();
            MatID = b.ReadInt32();  // Material ID?
            SkipBytes(b, 4);

            // Read the 4x4 transform matrix.
            Transform = new Matrix4x4
            {
                M11 = b.ReadSingle(),
                M12 = b.ReadSingle(),
                M13 = b.ReadSingle(),
                M14 = b.ReadSingle() * VERTEX_SCALE,
                M21 = b.ReadSingle(),
                M22 = b.ReadSingle(),
                M23 = b.ReadSingle(),
                M24 = b.ReadSingle() * VERTEX_SCALE,
                M31 = b.ReadSingle(),
                M32 = b.ReadSingle(),
                M33 = b.ReadSingle(),
                M34 = b.ReadSingle() * VERTEX_SCALE,
                M41 = b.ReadSingle(),
                M42 = b.ReadSingle(),
                M43 = b.ReadSingle(),
                M44 = b.ReadSingle(),
            };

            Pos = b.ReadVector3() * VERTEX_SCALE;
            Rot = b.ReadQuaternion();
            Scale = b.ReadVector3();
            
            // read the controller pos/rot/scale
            PosCtrlID = b.ReadInt32();
            RotCtrlID = b.ReadInt32();
            SclCtrlID = b.ReadInt32();

            Properties = b.ReadPString();
            // Good enough for now.

            SetLocalTransform();
        }
    }
}
