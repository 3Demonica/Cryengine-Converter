﻿using BinaryReaderExtensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace CgfConverter.CryEngineCore
{
    public class ChunkDataStream_800 : ChunkDataStream
    {
        private short starCitizenFlag = 0;

        public override void Read(BinaryReader b)
        {
            base.Read(b);

            Flags2 = b.ReadUInt32(); // another filler
            uint dataStreamType = b.ReadUInt32();
            DataStreamType = (DatastreamType)Enum.ToObject(typeof(DatastreamType), dataStreamType);
            NumElements = b.ReadUInt32(); // number of elements in this chunk

            if (_model.FileVersion == FileVersion.CryTek_3_5 || _model.FileVersion == FileVersion.CryTek_3_4)
            {
                BytesPerElement = b.ReadUInt32();
            }
            if (_model.FileVersion == FileVersion.CryTek_3_6)
            {
                BytesPerElement = (uint)b.ReadInt16();  // Star Citizen 2.0 is using an int16 here now.
                starCitizenFlag = b.ReadInt16();        // For Star Citizen files, this is 257.  Other known games (Hunt, Evolve, Prey) are 0.
            }

            SkipBytes(b, 8);

            // Now do loops to read for each of the different Data Stream Types.  If vertices, need to populate Vector3s for example.
            switch (DataStreamType)
            {
                #region case DataStreamTypeEnum.VERTICES:

                case DatastreamType.VERTICES:  // Ref is 0x00000000
                    Vertices = new Vector3[NumElements];

                    switch (BytesPerElement)
                    {
                        case 12:
                            for (int i = 0; i < NumElements; i++)
                            {
                                Vertices[i].x = b.ReadSingle();
                                Vertices[i].y = b.ReadSingle();
                                Vertices[i].z = b.ReadSingle();
                            }
                            break;
                        case 8:  // Prey files, and old Star Citizen files
                            for (int i = 0; i < NumElements; i++)
                            {
                                //uint bver = 0;
                                //float ver = 0;

                                // 2 byte floats.  Use the Half structure from TK.Math
                                //bver = b.ReadUInt16();
                                //ver = Byte4HexToFloat(bver.ToString("X8"));
                                //Vertices[i].x = ver;

                                //bver = b.ReadUInt16();
                                //ver = Byte4HexToFloat(bver.ToString("X8"));
                                //Vertices[i].y = ver; bver = b.ReadUInt16();

                                //bver = b.ReadUInt16();
                                //ver = Byte4HexToFloat(bver.ToString("X8"));
                                //Vertices[i].z = ver;

                                //bver = b.ReadUInt16();
                                //ver = Byte4HexToFloat(bver.ToString("X8"));
                                //Vertices[i].w = ver;
                                Half xshort = new Half();
                                xshort.bits = b.ReadUInt16();
                                Vertices[i].x = xshort.ToSingle();

                                Half yshort = new Half();
                                yshort.bits = b.ReadUInt16();
                                Vertices[i].y = yshort.ToSingle();

                                Half zshort = new Half();
                                zshort.bits = b.ReadUInt16();
                                Vertices[i].z = zshort.ToSingle();

                                b.ReadUInt16();
                            }
                            break;
                        case 16:
                            for (int i = 0; i < NumElements; i++)
                            {
                                Vertices[i].x = b.ReadSingle();
                                Vertices[i].y = b.ReadSingle();
                                Vertices[i].z = b.ReadSingle();
                                Vertices[i].w = b.ReadSingle(); // TODO:  Sometimes there's a W to these structures.  Will investigate.
                            }
                            break;
                    }
                    break;

                #endregion
                #region case DataStreamTypeEnum.INDICES:

                case DatastreamType.INDICES:  // Ref is 
                    Indices = new uint[NumElements];

                    if (BytesPerElement == 2)
                    {
                        for (int i = 0; i < NumElements; i++)
                        {
                            Indices[i] = b.ReadUInt16();
                        }
                    }
                    if (BytesPerElement == 4)
                    {
                        for (int i = 0; i < NumElements; i++)
                        {
                            Indices[i] = b.ReadUInt32();
                        }
                    }
                    break;

                #endregion
                #region case DataStreamTypeEnum.NORMALS:

                case DatastreamType.NORMALS:
                    Normals = new Vector3[NumElements];
                    for (int i = 0; i < NumElements; i++)
                    {
                        Normals[i].x = b.ReadSingle();
                        Normals[i].y = b.ReadSingle();
                        Normals[i].z = b.ReadSingle();
                    }
                    break;

                #endregion
                #region case DataStreamTypeEnum.UVS:

                case DatastreamType.UVS:
                    UVs = new UV[NumElements];
                    for (Int32 i = 0; i < NumElements; i++)
                    {
                        UVs[i].U = b.ReadSingle();
                        UVs[i].V = b.ReadSingle();
                    }
                    //Utils.Log(LogLevelEnum.Debug, "Offset is {0:X}", b.BaseStream.Position);
                    break;

                #endregion
                #region case DataStreamTypeEnum.TANGENTS:

                case DatastreamType.TANGENTS:
                    Tangents = new Tangent[NumElements, 2];
                    Normals = new Vector3[NumElements];
                    for (int i = 0; i < NumElements; i++)
                    {
                        switch (BytesPerElement)
                        {
                            case 0x10:
                                // These have to be divided by 32767 to be used properly (value between 0 and 1)
                                Tangents[i, 0].x = b.ReadInt16();
                                Tangents[i, 0].y = b.ReadInt16();
                                Tangents[i, 0].z = b.ReadInt16();
                                Tangents[i, 0].w = b.ReadInt16();

                                Tangents[i, 1].x = b.ReadInt16();
                                Tangents[i, 1].y = b.ReadInt16();
                                Tangents[i, 1].z = b.ReadInt16();
                                Tangents[i, 1].w = b.ReadInt16();

                                break;
                            case 0x08:
                                // These have to be divided by 127 to be used properly (value between 0 and 1)
                                // Tangent
                                Tangents[i, 0].w = b.ReadSByte() / 127;
                                Tangents[i, 0].x = b.ReadSByte() / 127;
                                Tangents[i, 0].y = b.ReadSByte() / 127;
                                Tangents[i, 0].z = b.ReadSByte() / 127;

                                // Binormal
                                Tangents[i, 1].w = b.ReadSByte() / 127;
                                Tangents[i, 1].x = b.ReadSByte() / 127;
                                Tangents[i, 1].y = b.ReadSByte() / 127;
                                Tangents[i, 1].z = b.ReadSByte() / 127;

                                // Calculate the normal based on the cross product of the tangents.
                                //Normals[i].x = (Tangents[i,0].y * Tangents[i,1].z - Tangents[i,0].z * Tangents[i,1].y);
                                //Normals[i].y = 0 - (Tangents[i,0].x * Tangents[i,1].z - Tangents[i,0].z * Tangents[i,1].x); 
                                //Normals[i].z = (Tangents[i,0].x * Tangents[i,1].y - Tangents[i,0].y * Tangents[i,1].x);
                                break;
                            default:
                                throw new Exception("Need to add new Tangent Size");
                        }
                    }
                    // Utils.Log(LogLevelEnum.Debug, "Offset is {0:X}", b.BaseStream.Position);
                    break;

                #endregion
                #region case DataStreamTypeEnum.COLORS:

                case DatastreamType.COLORS:
                    switch (BytesPerElement)
                    {
                        case 3:
                            Colors = new IRGBA[NumElements];
                            for (int i = 0; i < NumElements; i++)
                            {
                                Colors[i].r = b.ReadByte();
                                Colors[i].g = b.ReadByte();
                                Colors[i].b = b.ReadByte();
                                Colors[i].a = 255;
                            }
                            break;

                        case 4:
                            Colors = new IRGBA[NumElements];
                            for (int i = 0; i < NumElements; i++)
                            {
                                Colors[i] = b.ReadColor();
                            }
                            break;
                        default:
                            Utils.Log("Unknown Color Depth");
                            for (int i = 0; i < NumElements; i++)
                            {
                                SkipBytes(b, BytesPerElement);
                            }
                            break;
                    }
                    break;

                #endregion
                #region case DataStreamTypeEnum.VERTSUVS:

                case DatastreamType.VERTSUVS:
                    Vertices = new Vector3[NumElements];
                    Normals = new Vector3[NumElements];
                    Colors = new IRGBA[NumElements];
                    UVs = new UV[NumElements];
                    switch (BytesPerElement)  // new Star Citizen files
                    {
                        case 20:  // Dymek's code.  3 floats for vertex position, 4 bytes for normals, 2 halfs for UVs.  Normals are calculated from Tangents
                            for (int i = 0; i < NumElements; i++)
                            {
                                Vertices[i].x = b.ReadSingle();
                                Vertices[i].y = b.ReadSingle();
                                Vertices[i].z = b.ReadSingle();                  // For some reason, skins are an extra 1 meter in the z direction.

                                // Normals are stored in a signed byte, prob div by 127.
                                Normals[i].x = (float)b.ReadSByte() / 127;
                                Normals[i].y = (float)b.ReadSByte() / 127;
                                Normals[i].z = (float)b.ReadSByte() / 127;
                                b.ReadSByte(); // Should be FF.

                                Half uvu = new Half();
                                uvu.bits = b.ReadUInt16();
                                UVs[i].U = uvu.ToSingle();

                                Half uvv = new Half();
                                uvv.bits = b.ReadUInt16();
                                UVs[i].V = uvv.ToSingle();

                                //bver = b.ReadUInt16();
                                //ver = Byte4HexToFloat(bver.ToString("X8"));
                                //UVs[i].U = ver;

                                //bver = b.ReadUInt16();
                                //ver = Byte4HexToFloat(bver.ToString("X8"));
                                //UVs[i].V = ver;
                            }
                            break;
                        case 16:   // Dymek updated
                            if (starCitizenFlag == 257)
                            {
                                for (int i = 0; i < NumElements; i++)
                                {
                                    Vertices[i].x = b.ReadCryHalf();
                                    Vertices[i].y = b.ReadCryHalf();
                                    Vertices[i].z = b.ReadCryHalf();
                                    Vertices[i].w = b.ReadCryHalf();

                                    // Read a Quat, convert it to vector3
                                    Vector4 quat = new Vector4();
                                    quat.x = b.ReadSByte() / 127.5f;
                                    quat.y = b.ReadSByte() / 127.5f;
                                    quat.z = b.ReadSByte() / 127.5f;
                                    quat.w = b.ReadSByte() / 127.5f;
                                    Normals[i].x = (2 * (quat.x * quat.z + quat.y * quat.w));
                                    Normals[i].y = (2 * (quat.y * quat.z - quat.x * quat.w));
                                    Normals[i].z = (2 * (quat.z * quat.z + quat.w * quat.w)) - 1;

                                    // UVs ABSOLUTELY should use the Half structures.
                                    Half uvu = new Half();
                                    uvu.bits = b.ReadUInt16();
                                    UVs[i].U = uvu.ToSingle();

                                    Half uvv = new Half();
                                    uvv.bits = b.ReadUInt16();
                                    UVs[i].V = uvv.ToSingle();
                                }
                            }
                            else
                            {
                                #region Legacy version using Halfs (Also Hunt models)
                                for (int i = 0; i < NumElements; i++)
                                {
                                    Half xshort = new Half();
                                    xshort.bits = b.ReadUInt16();
                                    Vertices[i].x = xshort.ToSingle();

                                    Half yshort = new Half();
                                    yshort.bits = b.ReadUInt16();
                                    Vertices[i].y = yshort.ToSingle();

                                    Half zshort = new Half();
                                    zshort.bits = b.ReadUInt16();
                                    Vertices[i].z = zshort.ToSingle();

                                    Half xnorm = new Half();
                                    xnorm.bits = b.ReadUInt16();
                                    Normals[i].x = xnorm.ToSingle();

                                    Half ynorm = new Half();
                                    ynorm.bits = b.ReadUInt16();
                                    Normals[i].y = ynorm.ToSingle();

                                    Half znorm = new Half();
                                    znorm.bits = b.ReadUInt16();
                                    Normals[i].z = znorm.ToSingle();

                                    Half uvu = new Half();
                                    uvu.bits = b.ReadUInt16();
                                    UVs[i].U = uvu.ToSingle();

                                    Half uvv = new Half();
                                    uvv.bits = b.ReadUInt16();
                                    UVs[i].V = uvv.ToSingle();
                                }
                                #endregion
                            }
                            break;
                        default:
                            Utils.Log("Unknown VertUV structure");
                            for (Int32 i = 0; i < NumElements; i++)
                            {
                                SkipBytes(b, BytesPerElement);
                            }
                            break;
                    }
                    break;
                #endregion
                #region case DataStreamTypeEnum.BONEMAP:
                case DatastreamType.BONEMAP:
                    SkinningInfo skin = GetSkinningInfo();
                    skin.HasBoneMapDatastream = true;
                    skin.BoneMapping = new List<MeshBoneMapping>();

                    switch (BytesPerElement)
                    {
                        case 8:
                            for (int i = 0; i < NumElements; i++)
                            {
                                MeshBoneMapping tmpMap = new MeshBoneMapping();
                                tmpMap.BoneIndex = new int[4];
                                tmpMap.Weight = new int[4];

                                for (int j = 0; j < 4; j++)         // read the 4 bone indexes first
                                {
                                    tmpMap.BoneIndex[j] = b.ReadByte();

                                }
                                for (int j = 0; j < 4; j++)           // read the weights.
                                {
                                    tmpMap.Weight[j] = b.ReadByte();
                                }
                                skin.BoneMapping.Add(tmpMap);
                            }
                            break;
                        case 12:
                            for (int i = 0; i < NumElements; i++)
                            {
                                MeshBoneMapping tmpMap = new MeshBoneMapping();
                                tmpMap.BoneIndex = new int[4];
                                tmpMap.Weight = new int[4];

                                for (int j = 0; j < 4; j++)         // read the 4 bone indexes first
                                {
                                    tmpMap.BoneIndex[j] = b.ReadUInt16();

                                }
                                for (int j = 0; j < 4; j++)           // read the weights.
                                {
                                    tmpMap.Weight[j] = b.ReadByte();
                                }
                                skin.BoneMapping.Add(tmpMap);
                            }
                            break;

                        default:
                            Utils.Log("Unknown BoneMapping structure");
                            break;
                    }

                    break;

                #endregion
                #region case DataStreamTypeEnum.QTangents
                case DatastreamType.QTANGENTS:
                    Tangents = new Tangent[NumElements, 2];
                    Normals = new Vector3[NumElements];
                    for (int i = 0; i < NumElements; i++)
                    {
                        Tangents[i, 0].w = b.ReadSByte() / 127;
                        Tangents[i, 0].x = b.ReadSByte() / 127;
                        Tangents[i, 0].y = b.ReadSByte() / 127;
                        Tangents[i, 0].z = b.ReadSByte() / 127;

                        // Binormal
                        Tangents[i, 1].w = b.ReadSByte() / 127;
                        Tangents[i, 1].x = b.ReadSByte() / 127;
                        Tangents[i, 1].y = b.ReadSByte() / 127;
                        Tangents[i, 1].z = b.ReadSByte() / 127;

                        // Calculate the normal based on the cross product of the tangents.
                        Normals[i].x = (Tangents[i, 0].y * Tangents[i, 1].z - Tangents[i, 0].z * Tangents[i, 1].y);
                        Normals[i].y = 0 - (Tangents[i, 0].x * Tangents[i, 1].z - Tangents[i, 0].z * Tangents[i, 1].x);
                        Normals[i].z = (Tangents[i, 0].x * Tangents[i, 1].y - Tangents[i, 0].y * Tangents[i, 1].x);
                    }
                    break;
                #endregion // Prey normals?
                #region default:

                default:
                    Utils.Log(LogLevelEnum.Debug, "***** Unknown DataStream Type *****");
                    break;

                    #endregion
            }
        }
    }
}
