using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.BackendServer.Data.Interfaces
{
   public interface ITrackableEntity
    {
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
