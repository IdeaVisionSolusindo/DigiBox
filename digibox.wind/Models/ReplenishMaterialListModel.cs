using digibox.services.Models;
using digibox.wind.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace digibox.wind.Models
{
    public class ReplenishMaterialListModel:BaseViewModel
    {
        public Guid id { get; set; }
        public Guid materialid { get; set; } //for material id
        public string  partno { get; set; }
        public string materialname { get; set; }
        public decimal amount { get; set; }
        public decimal maximum { get; set; }
        public decimal replenishamount { get; set; }
        public string unit { get; set; }
        public string rfidCode { get; set; }

        public delegate void PrintData(Guid id);
        public delegate void RemoveData(Guid id);

        public ReplenishMaterialListModel()
        {
            PrintCommand = new RelayCommand((s) => Print(s), (y)=>CanPrint(y));
            RemoveCommand = new RelayCommand((s) => Remove(s));
        }

        private bool CanPrint(object y)
        {
            return IsSaved;
        }

        //declare event of type delegate
        public event PrintData printDataEvent;
        public event RemoveData removeDataEvent;

        public decimal ReplenishAmount
        {
            get { return replenishamount; }
            set { replenishamount = value;
                OnPropertyChanged("ReplenishAmount");
            }
        }


        private bool _isSaved;

        public bool IsSaved
        {
            get { return _isSaved; }
            set { _isSaved = value;
                OnPropertyChanged("IsSaved");
            }
        }

        private void Print(object s)
        {
            Guid id = new Guid(s.ToString());
            printDataEvent(id);
        }

        private void Remove(object s)
        {
            Guid id = new Guid(s.ToString());
            removeDataEvent(id);
        }
        public ICommand PrintCommand
        {
            get;
            private set;
        }

        public ICommand RemoveCommand
        {
            get;
            private set;
        }
       

    }

    public class ReplenishListModel : ReplenishModelListModel
    {
        public delegate void EditData(Guid id);
        public delegate void RemoveData(Guid id);
        public delegate void ConfirmData(Guid id);
        public ReplenishListModel()
        {
            EditCommand = new RelayCommand((s) => Edit(s), (y)=>isDraft(y));
            RemoveCommand = new RelayCommand((s) => Remove(s), (y) => isDraft(y));
            ConfirmCommand = new RelayCommand((s) => Confirm(s), (y) => isDraft(y));
        }

        private bool isDraft(object y)
        {
            return isDeletable;
        }

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

        public ICommand ConfirmCommand
        {
            get;
            private set;
        }

        public bool isDeletable { get; set; }

        //declare event of type delegate
        public event EditData editDataEvent;
        public event RemoveData removeDataEvent;
        public event ConfirmData confirmDataEvent;

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

        private void Confirm(object s)
        {
            Guid id = new Guid(s.ToString());
            confirmDataEvent(id);
        }
    }
}
