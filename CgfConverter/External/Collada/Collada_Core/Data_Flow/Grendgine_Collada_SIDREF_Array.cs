using System;
using System.Xml;
using System.Xml.Serialization;
namespace grendgine_collada
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class Grendgine_Collada_SIDREF_Array : Grendgine_Collada_String_Array_String
    {
        [XmlAttribute("id")]
        public string ID;

        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("count")]
        public int Count;
    }
}

