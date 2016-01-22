using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CallApp.Models
{
    public class Number
    {
        public long Id { get; set; }

        [Required, Index]
        public string PhoneNumber { get; set; }

    }
}