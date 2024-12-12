using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Services
{
    public class ServiceManager : IServiceManager
    {
        public ITournamentService TournamentService;

        public IGameService GameService;
    }
}
