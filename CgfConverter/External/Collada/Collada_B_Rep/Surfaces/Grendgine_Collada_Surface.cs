using System;
using System.Xml;
using System.Xml.Serialization;
namespace grendgine_collada
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class Grendgine_Collada_Surface
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("sid")]
        public string sID;

        // ggerber 1.4.1 attribue
        [XmlAttribute("type")]
        public string Type;

        [XmlElement(ElementName = "cone")]
        public Grendgine_Collada_Cone Cone;

        [XmlElement(ElementName = "plane")]
        public Grendgine_Collada_Plane Plane;

        [XmlElement(ElementName = "cylinder")]
        public Grendgine_Collada_Cylinder_B_Rep Cylinder;

        [XmlElement(ElementName = "nurbs_surface")]
        public Grendgine_Collada_Nurbs_Surface Nurbs_Surface;

        [XmlElement(ElementName = "sphere")]
        public Grendgine_Collada_Sphere Sphere;

        [XmlElement(ElementName = "torus")]
        public Grendgine_Collada_Torus Torus;

        [XmlElement(ElementName = "swept_surface")]
        public Grendgine_Collada_Swept_Surface Swept_Surface;

        [XmlElement(ElementName = "orient")]
        public Grendgine_Collada_Orient[] Orient;

        [XmlElement(ElementName = "origin")]
        public Grendgine_Collada_Origin Origin;

        //ggerber 1.4.1
        [XmlElement(ElementName = "init_from")]
        public Grendgine_Collada_Init_From Init_From;

    }
}

