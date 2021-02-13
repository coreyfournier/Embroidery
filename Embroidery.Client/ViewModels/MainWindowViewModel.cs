using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(FakeDatabase db)
        {
            List = new TodoListViewModel(db.GetItems());
        }

        public TodoListViewModel List { get; }
    }
}
