using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.Models
{
    public class File
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(128), Required]
        public string Name { get; set; } = "";

        [MaxLength(256), Required]
        public string Path { get; set; } = "";

        public int SizeInKb { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public List<Tag>? Tags { get; set; }

        public byte[]? ImageThumbnail { get; set; }
    }
}
