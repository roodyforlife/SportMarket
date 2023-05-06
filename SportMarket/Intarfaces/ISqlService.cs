using Microsoft.Data.SqlClient;
using SportMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Intarfaces
{
    public interface ISqlService
    {
        public RequestViewModel SendRequest(string request);
    }
}
