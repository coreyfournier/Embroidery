using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.Models
{
    public class FileTagRelationship
    {
        public int FileId { get; set; }
        public File File { get; set; }

        public int TagId { get; set; }

        public Tag Tag { get; set; }
    }
}
