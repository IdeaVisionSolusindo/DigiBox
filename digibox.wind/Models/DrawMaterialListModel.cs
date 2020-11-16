using digibox.services.Models;
using digibox.wind.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Input;
using System.Windows.Media;

namespace digibox.wind.Models
{
    public class DrawMaterialListModel:BaseViewModel
    {
        public Guid id { get; set; }
        public Guid materialid { get; set; } //for material id
        public string  partno { get; set; }
        public string materialname { get; set; }
        public decimal amount { get; set; }
        public decimal maximum { get; set; }
        public decimal drawamount { get; set; }
        public string unit { get; set; }
        public string rfidCode { get; set; }

        public delegate void PrintData(Guid id);
        public delegate void RemoveData(Guid id);

        public DrawMaterialListModel()
        {
            PrintCommand = new RelayCommand((s) => Print(s));
            RemoveCommand = new RelayCommand((s) => Remove(s));

        }

        //declare event of type delegate
        public event PrintData printDataEvent;
        public event RemoveData removeDataEvent;

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

        public decimal DrawAmount {
            get
            {
                return drawamount;
            }
            set
            {
                drawamount = value;
                OnPropertyChanged("DrawAmount");
            }
        }

    }

    public class DrawListModel :DrawBaseModel
    {
        public delegate void EditData(Guid id);
        public delegate void RemoveData(Guid id);
        public DrawListModel()
        {
            EditCommand = new RelayCommand((s) => Edit(s), (y)=>isDraft(y));
            RemoveCommand = new RelayCommand((s) => Remove(s), (y) => isDraft(y));
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

        public string Items { get; set; }
        public bool isDeletable { get; set; }

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
