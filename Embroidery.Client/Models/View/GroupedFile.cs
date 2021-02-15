using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.Models.View
{
    public class GroupedFile
    {
        public int FirstFileId { get; set; }

        public string CleanName { get; set; }

        public int TotalLikeFiles { get; set; }
    }
}
