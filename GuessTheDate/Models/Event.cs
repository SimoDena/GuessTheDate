using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GuessTheDate.Models
{
    [Serializable()]
    [XmlType(TypeName ="event")]
    public class Event
    {
        [XmlElement("date")]
        public string Date { get; set; }
        [XmlElement("description")]
        public string Description { get; set; }
    }
}
