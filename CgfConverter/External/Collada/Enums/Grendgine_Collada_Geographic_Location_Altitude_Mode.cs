using System;
using System.Xml.Serialization;

namespace grendgine_collada
{
    [Serializable]
    [XmlType(Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
    public enum Grendgine_Collada_Geographic_Location_Altitude_Mode
    {
        absolute,
        relativeToGround
    }
}

