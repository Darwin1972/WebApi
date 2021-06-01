using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data.Models
{
    public class Thingslog
    {
        public Thingslog()
        {
        }

        [Key]
        [Required]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Tenant { get; set; }
        public int Status { get; set; }
        public string Log { get; set; }
    }

}
