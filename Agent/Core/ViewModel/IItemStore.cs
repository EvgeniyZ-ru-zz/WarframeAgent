using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModel
{
    public interface IItemStore
    {
        ItemViewModel GetItemById(string id);
    }
}
