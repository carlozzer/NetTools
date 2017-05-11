using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TC.FrameWork.Sharepoint13.Caml
{
    [DataContract(Namespace="")]
    public class CamlContainer
    {
        [DataMember] public string Query { get; set; }
    }
}
