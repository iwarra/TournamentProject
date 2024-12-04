using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Dto
{
    public record TournamentDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate
        {
            get
            {
                return StartDate.AddMonths(3);
            }
        }
        public List<GameDto>? Games { get; set; }
    }
}
