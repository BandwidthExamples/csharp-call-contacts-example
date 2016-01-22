using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CallApp.Models
{
    public class Contact
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, Index]
        public string PhoneNumber { get; set; }
    }
}