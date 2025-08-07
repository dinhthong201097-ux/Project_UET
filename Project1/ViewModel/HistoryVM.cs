using UII.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UII.ViewModel
{
    public class HistoryVM : BaseViewModel
    {
        private readonly PageModel _pageModel;

        public HistoryVM()
        {
            _pageModel = new PageModel();
        }


    }
}
