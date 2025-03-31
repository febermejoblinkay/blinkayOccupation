using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlinkayOccupation.Application.Models
{
    public class BlobStorageSettings
    {
        public string BlobEndpoint { get; set; }
        public string ContainerName { get; set; }
    }
}
