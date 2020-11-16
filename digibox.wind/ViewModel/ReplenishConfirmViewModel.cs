using Autofac;
using ControlzEx.Standard;
using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.Interfaces;
using digibox.services.Services.interfaces;
using digibox.wind.Models;
using digibox.wind.Modules;
using digibox.wind.Services;
using digibox.wind.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Xml.Schema;

namespace digibox.wind.ViewModel
{
    public class ReplenishConfirmViewModel:BaseViewModel
    {

        private ObservableCollection<ReplenishMaterialListModel> _replenishContent = new ObservableCollection<ReplenishMaterialListModel>();
        private readonly IUserRepository _user;
        private IMaterialRepository _material;
        private readonly IMaterialServices _materialServices;
        private IReplenishRepository _replenish;
        private readonly IReplenishDetailRepository _replenishDetail;
        private readonly IMaterialPriceRepository _materialPrice;
        private string _RFIDCode;

        public ReplenishConfirmViewModel(IReplenishRepository replenish, IReplenishDetailRepository replenishDetail,
            IMaterialRepository material, IMaterialServices materialServices, IMaterialPriceRepository materialPrice,
            IUserRepository user)
        {
            _user = user;
            _material = material;
            _materialServices = materialServices;
            _replenish = replenish;
            _replenishDetail = replenishDetail;
            _materialPrice = materialPrice;

            //initialize property
            RFIDCode = "";

            var currentuser = myGlobal.currentUser;
            UserID = currentuser.id;
            Name = currentuser.nama;
            CurrentDate = DateTime.Now;


            //initialize command
            ConfirmCommand = new RelayCommand((x) => ConfirmData(x), (y) => CanConfirmData(y));
            AddItemCommand = new RelayCommand((x) => AddItem(x), (y) => CanAddItem(y));
        }

        public string RFIDCode
        {
            get { return _RFIDCode; }
            set { _RFIDCode = value;
                OnPropertyChanged("RFIDCode");
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; 
                OnPropertyChanged("Name");
            }
        }

        private DateTime _currentDate;
        public DateTime CurrentDate {
            get { return _currentDate; }
            set { _currentDate = value;
                OnPropertyChanged("CurrentDate");
            }
        }

        private Guid _id;

        public Guid ID
        {
            get { return _id; }
            set { _id = value; }
        }


        private Guid _userID;

        public Guid UserID
        {
            get { return _userID; }
            set { _userID = value; 
                OnPropertyChanged("ID");
            }
        }

        public bool isEdit { get; set; }

        public ObservableCollection<ReplenishMaterialListModel> ReplenishContent
        {
            get { 
                    return _replenishContent; 
            }
            set { _replenishContent = value; 
                OnPropertyChanged("ReplenishContent");
            }
        }


        private bool _isConfirmed;

        public bool IsConfirmed
        {
            get { return _isConfirmed; }
            set { _isConfirmed = value; 
                OnPropertyChanged("IsConfirmed");
            }
        }


        public ICommand AddItemCommand
        {
            get;
            private set;
        }
        public ICommand ConfirmCommand
        {
            get;
            private set;
        }


        private bool CanAddItem(object s)
        {
            return RFIDCode.Length != 0;
        }

        private void AddItem(object s)
        {

            var dta = ReplenishContent.Where(x => x.rfidCode == RFIDCode);
            if (dta.Count() != 0)
            {
                //check kalau replenish amount melebih batas isi, beep;
                var itm = dta.FirstOrDefault();
                var maxisi = itm.maximum - itm.amount;
                if (itm.ReplenishAmount >= maxisi)
                {
                    return;
                }
                itm.ReplenishAmount = itm.replenishamount + 1;
                return;
            }
            RFIDCode = String.Empty;
        }

        private bool CanConfirmData(object y)
        {
            return !IsConfirmed;
        }

        private void ConfirmData(object x)
        {
            try
            {

                var invalid = ReplenishContent.Where(yx => yx.replenishamount == 0).Count();
                if (invalid!=0)
                {
                    MessageBox.Show("Invalid Receive Data");
                    return;
                }

                var dta = _replenish.FindById(ID);
                dta.updatedby = myGlobal.currentUser.nama;
                dta.status = ReplenishStatus.APPROVED;
                //update detail

                var detail = (from rd in ReplenishContent
                              select new ReplenishDetailReceiveModel()
                              {
                                  amount = rd.amount,
                                  id = rd.id,
                                  materialid = rd.materialid,
                                  replenishid = ID,
                                  receiveamount = rd.replenishamount,
                                  rfidcode = rd.rfidCode
                              }).ToList();

                _replenish.Received(dta, detail);
                MessageBox.Show("Data is Confirmed");
                IsConfirmed = true;
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void LoadDetail()
        {
            var rped = _replenishDetail.GetByReplenishId(ID).ToList();
            var materialinfo = _materialServices.GetMaterialCurrentPrice().ToList();
            var dta = (from d in rped
                       join m in materialinfo on d.materialid equals m.id
                       select new ReplenishMaterialListModel()
                       {
                           id=d.id,
                           materialid = d.id,
                           replenishamount = 0,
                           amount = d.amount,
                           materialname = m.name,
                           maximum = m.maxstock ?? 0,
                           partno = m.partno,
                           rfidCode = d.rfidcode,
                           unit = m.unit,
                           IsSaved = isEdit

                       }).ToList();
            foreach(var itm in dta)
            {
                itm.printDataEvent += Itm_printDataEvent;
                itm.removeDataEvent += Itm_removeDataEvent;
            }
            ReplenishContent = new ObservableCollection<ReplenishMaterialListModel>(dta);
        }

        private void Itm_removeDataEvent(Guid id)
        {
            var itm = ReplenishContent.Where(x => x.id == id).FirstOrDefault();
            ReplenishContent.Remove(itm);
        }

        private void Itm_printDataEvent(Guid id)
        {

            var itm = ReplenishContent.Where(x => x.id == id).FirstOrDefault();
            var rfidcode = itm.rfidCode;

            var container = iocInit.ConfigureContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                PrintLabelService prn = scope.Resolve<PrintLabelService>();
                //prn.ID = SelectedOrder.id;
                prn.PrintLabel(itm.id);
            }

        }
    }
}
