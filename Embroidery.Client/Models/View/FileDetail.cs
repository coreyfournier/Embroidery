using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.Models.View
{
    public class FileDetail
    {
        public GroupedFile GroupedFile { get; set; }

        public IEnumerable<SimpleFile> SimpleFiles { get; set; }
    }
}
