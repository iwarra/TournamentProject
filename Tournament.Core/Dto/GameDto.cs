using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Dto
{
    public record GameDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
    }
}
