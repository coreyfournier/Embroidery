using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.Models
{
    /// <summary>
    /// Key value pairs for various settings
    /// </summary>
    [Index(nameof(Key),IsUnique = true)]
    public class Setting
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(75), Required]
        public string Key { get; set; }

        [MaxLength(2058), Required]
        public string Value { get; set; }

        [MaxLength(100)]
        public string Type { get; set; } = null;

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
