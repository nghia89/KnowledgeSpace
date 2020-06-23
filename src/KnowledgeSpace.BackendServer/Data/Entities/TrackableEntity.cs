using KnowledgeSpace.BackendServer.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSpace.BackendServer.Data.Entities
{
    public class TrackableEntity : ITrackableEntity
    {
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
