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
        public string Title { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate
        {
            get
            {
                return StartDate.AddMonths(3);
            }
        }
        public List<GameDto>? Games { get; init; }
    }
}
