﻿using System;
using System.IO;
using System.Reflection;
using System.Text;
using static Extensions.FileHandlingExtensions;

namespace CgfConverter
{
    public partial class Wavefront
    {
        public void WriteMaterial(CryEngine cryEngine)
        {
            if (cryEngine.Materials == null)
            {
                Utils.Log(LogLevelEnum.Debug, "No materials loaded");
                return;
            }

            if (!this.OutputFile_Material.Directory.Exists)
                this.OutputFile_Material.Directory.Create();

            using (StreamWriter file = new StreamWriter(this.OutputFile_Material.FullName))
            {
                file.WriteLine("# cgf-converter .mtl export version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
                file.WriteLine("#");
                foreach (CryEngineCore.Material material in cryEngine.Materials)
                {
                    string MatName = material.Name;
                    if (Args.PrefixMaterialNames)
                        MatName = material.SourceFileName + "_" + MatName;
#if DUMP_JSON
                    File.WriteAllText(String.Format("_material-{0}.json", MatName.Replace(@"/", "").Replace(@"\", "")), material.ToJSON());
#endif
                    file.WriteLine("newmtl {0}", MatName);
                    if (material.Diffuse != null)
                    {
                        file.WriteLine("Ka {0:F6} {1:F6} {2:F6}", material.Diffuse.Red, material.Diffuse.Green, material.Diffuse.Blue);    // Ambient
                        file.WriteLine("Kd {0:F6} {1:F6} {2:F6}", material.Diffuse.Red, material.Diffuse.Green, material.Diffuse.Blue);    // Diffuse
                    }
                    else
                    {
                        Utils.Log(LogLevelEnum.Debug, "Skipping Diffuse for {0}", MatName);
                    }
                    if (material.Specular != null)
                    {
                        file.WriteLine("Ks {0:F6} {1:F6} {2:F6}", material.Specular.Red, material.Specular.Green, material.Specular.Blue); // Specular
                        file.WriteLine("Ns {0:F6}", material.Shininess / 255D);                                                            // Specular Exponent
                    }
                    else
                    {
                        Utils.Log(LogLevelEnum.Debug, "Skipping Specular for {0}", MatName);
                    }
                    file.WriteLine("d {0:F6}", material.Opacity);                                                                          // Dissolve

                    file.WriteLine("illum 2");  // Highlight on. This is a guess.

                    // Phong materials

                    // 0. Color on and Ambient off
                    // 1. Color on and Ambient on
                    // 2. Highlight on
                    // 3. Reflection on and Ray trace on
                    // 4. Transparency: Glass on, Reflection: Ray trace on
                    // 5. Reflection: Fresnel on and Ray trace on
                    // 6. Transparency: Refraction on, Reflection: Fresnel off and Ray trace on
                    // 7. Transparency: Refraction on, Reflection: Fresnel on and Ray trace on
                    // 8. Reflection on and Ray trace off
                    // 9. Transparency: Glass on, Reflection: Ray trace off
                    // 10. Casts shadows onto invisible surfaces

                    if (!Args.NoTextures)
                    {
                        foreach (CryEngineCore.Material.Texture texture in material.Textures)
                        {
                            StringBuilder textureFile = new StringBuilder(ResolveTexFile(texture.File, Args.DataDir));

                            // TODO: More filehandling here

                            if (this.Args.PngTextures)
                                textureFile = textureFile.Replace(".dds", ".png");
                            else if (this.Args.TgaTextures)
                                textureFile = textureFile.Replace(".dds", ".tga");
                            else if (this.Args.TiffTextures)
                                textureFile = textureFile.Replace(".dds", ".tif");

                            switch (texture.Map)
                            {
                                case CryEngineCore.Material.Texture.MapTypeEnum.Diffuse:
                                    file.WriteLine("map_Kd {0}", textureFile.ToString());
                                    break;

                                case CryEngineCore.Material.Texture.MapTypeEnum.Specular:
                                    file.WriteLine("map_Ks {0}", textureFile.ToString());
                                    file.WriteLine("map_Ns {0}", textureFile.ToString());
                                    break;

                                case CryEngineCore.Material.Texture.MapTypeEnum.Bumpmap:
                                case CryEngineCore.Material.Texture.MapTypeEnum.Detail:
                                    file.WriteLine("map_bump {0}", textureFile.ToString());
                                    break;

                                case CryEngineCore.Material.Texture.MapTypeEnum.Heightmap:
                                    file.WriteLine("disp {0}", textureFile.ToString());
                                    break;

                                case CryEngineCore.Material.Texture.MapTypeEnum.Decal:
                                    file.WriteLine("decal {0}", textureFile.ToString());
                                    break;

                                case CryEngineCore.Material.Texture.MapTypeEnum.SubSurface:
                                    file.WriteLine("map_Ns {0}", textureFile.ToString());
                                    break;

                                case CryEngineCore.Material.Texture.MapTypeEnum.Opacity:
                                    file.WriteLine("map_d {0}", textureFile.ToString());
                                    break;

                                case CryEngineCore.Material.Texture.MapTypeEnum.Custom:
                                case CryEngineCore.Material.Texture.MapTypeEnum.BlendDetail:
                                case CryEngineCore.Material.Texture.MapTypeEnum.Environment:
                                default:
                                    break;
                            }
                        }
                    }

                    file.WriteLine();
                }
            }
        }
    }
}