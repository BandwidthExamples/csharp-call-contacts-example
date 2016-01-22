using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CallApp.Models
{
    public class Number
    {
        public long Id { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required, Index(IsUnique = true)]
        public string Type { get; set; }
    }
}