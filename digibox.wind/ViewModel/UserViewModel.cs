using digibox.wind.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace digibox.wind.ViewModel
{
    public class UserViewModel:BaseViewModel
    {

        public delegate void EditData(Guid id);
        public delegate void RemoveData(Guid id);
        public ICommand EditCommand
        {
            get;
            private set;
        }

        public ICommand RemoveCommand
        {
            get;
            private set;
        }

        public UserViewModel()
        {
            EditCommand = new RelayCommand((s) => Edit(s));
            RemoveCommand = new RelayCommand((s) => Remove(s));
        }


        //declare event of type delegate
        public event EditData editDataEvent;
        public event RemoveData removeDataEvent;

        private void Edit(object s)
        {
            Guid id = new Guid(s.ToString());
            editDataEvent(id);
        }

        private void Remove(object s)
        {
            Guid id = new Guid(s.ToString());
            removeDataEvent(id);
        }
    }
}
