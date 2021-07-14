using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Interfaces
{
    public interface IOrderReserverService
    {
        Task PlaceOrderAsync(Dictionary<string, int> items);
    }
}
