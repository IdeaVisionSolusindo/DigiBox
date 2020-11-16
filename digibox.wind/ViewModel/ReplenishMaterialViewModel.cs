using Autofac;
using ControlzEx.Standard;
using digibox.data;
using digibox.services.Repositories.Interfaces;
using digibox.services.Services.interfaces;
using digibox.wind.Models;
using digibox.wind.Modules;
using digibox.wind.Services;
using digibox.wind.View;
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
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Xml.Schema;

namespace digibox.wind.ViewModel
{
    public class ReplenishMaterialViewModel:BaseViewModel
    {

        private ObservableCollection<ReplenishMaterialListModel> _replenishContent = new ObservableCollection<ReplenishMaterialListModel>();
        private readonly IUserRepository _user;
        private IMaterialRepository _material;
        private readonly IMaterialServices _materialServices;
        private IReplenishRepository _replenish;
        private readonly IReplenishDetailRepository _replenishDetail;
        private readonly IMaterialPriceRepository _materialPrice;
        private string _partNo;

        public ReplenishMaterialViewModel(IReplenishRepository replenish, IReplenishDetailRepository replenishDetail,
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
            PartNo = "";

            var currentuser = myGlobal.currentUser;
            UserID = currentuser.id;
            Name = currentuser.nama;
            CurrentDate = DateTime.Now;


            //initialize command
            AddItemCommand = new RelayCommand((x) => AddItem(x), (y) => CanAddItem(y));
            SaveCommand = new RelayCommand((x) => SaveData(x), (y) => CanSaveData(y));
            ConfirmCommand = new RelayCommand((x) => ConfirmData(x), (y) => CanSaveData(y));

        }

        private void ConfirmData(object x)
        {
            SaveData(x);
            var container = iocInit.ConfigureContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                var _replenishCornfimViewModel = scope.Resolve<ReplenishConfirmViewModel>();
                var _replenishconfirmview = scope.Resolve<ReplenishConfirmView>();
                _replenishconfirmview.DataContext = _replenishCornfimViewModel;

                var rep = _replenish.FindById(ID);
                var usr = _user.FindById(rep.collectorid ?? Guid.Empty);
                _replenishCornfimViewModel.UserID = rep.id;
                _replenishCornfimViewModel.ID = ID;
                _replenishCornfimViewModel.CurrentDate = rep.indate ?? DateTime.Now;
                _replenishCornfimViewModel.Name = usr.name;
                _replenishCornfimViewModel.isEdit = true;
                _replenishCornfimViewModel.LoadDetail();
                //Caller.CurrentViewModel = _replenishCornfimViewModel;
                //Caller.CurrentView = _replenishconfirmview;
            }
        }

        public string PartNo
        {
            get { return _partNo; }
            set { _partNo = value;
                OnPropertyChanged("PartNo");
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

        private bool _issaved;

        public bool IsSaved
        {
            get { return _issaved; }
            set { 
                
                _issaved = value;
                if (_issaved)
                {
                    foreach(var itm in ReplenishContent)
                    {
                        itm.IsSaved = _issaved;
                    }
                }
                OnPropertyChanged("IsSaved");
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



        public ICommand AddItemCommand
        {
            get;
            private set;
        }
        public ICommand SaveCommand
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
            return PartNo.Length != 0;
        }

        private void AddItem(object s)
        {
            //check apakah material tersebut milik user
            var material = _material.GetMaterialByCollector(myGlobal.currentUser.id);
            var ismine = material.Where(x => x.partno == PartNo).Count()!=0;
            if (!ismine)
            {
                SystemSounds.Beep.Play();
                MessageBox.Show("This Item belong to other collector");
                return;
            }

            //check apakah material sudah ada dalam list;
            var inList = ReplenishContent.Where(x => x.partno == PartNo).ToList();
            if (inList.Count() != 0)
            {
                var item = inList.FirstOrDefault();

                decimal total = item.amount+item.ReplenishAmount + 1;
                if (total >= item.maximum)
                {
                    SystemSounds.Beep.Play();
                    return;
                }
                item.ReplenishAmount = item.ReplenishAmount + 1;
                PartNo = String.Empty;
                return;
            }
            //Adding to list in the 
            var thismaterial = material.Where(x => x.partno == PartNo).FirstOrDefault();
            var materialinfo = _materialServices.GetMaterialCurrentPrice().Where(x => x.id == thismaterial.id).FirstOrDefault();

            CultureInfo CI = new CultureInfo("id-ID");
            System.Globalization.Calendar Cal = CI.Calendar;
            DateTime currentdate = DateTime.Now;
            int week = Cal.GetWeekOfYear(currentdate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            string precode = string.Format("{0:yy}{1}", currentdate, week);

            //decimal? replamount = (materialinfo.maxstock - materialinfo.currentstock);

            ReplenishMaterialListModel content = new ReplenishMaterialListModel()
            {
                id = Guid.NewGuid(),
                materialid = thismaterial.id,
                materialname = thismaterial.name,
                maximum = materialinfo.maxstock ?? 0,
                amount = materialinfo.currentstock ?? 0,
                partno = thismaterial.partno,
                unit = thismaterial.unit,
                replenishamount = 1,
                rfidCode = $"{precode}{thismaterial.partno}",
                IsSaved = false   
            };


            content.printDataEvent += Itm_printDataEvent;
            content.removeDataEvent += Itm_removeDataEvent;
            ReplenishContent.Add(content);
            PartNo = String.Empty;
        }

        private bool CanSaveData(object y)
        {
            return ReplenishContent.Count() != 0;
        }

        private void SaveData(object x)
        {
            try
            {
                if (!isEdit)
                {
                    ttreplenish rep = new ttreplenish()
                    {
                        collectorid = UserID,
                        createdat = DateTime.Now,
                        createdby = Name,
                        id = Guid.NewGuid(),
                        indate = CurrentDate,
                        no = String.Format("{0:yyyymmhhsssff}", CurrentDate),
                        status = ReplenishStatus.DRAFT
                    };

                    var result = _replenish.Create(rep);
                    ID = result.id;
                    List<tdreplenish> details = new List<tdreplenish>();
                    //detail;
                    foreach (var itm in ReplenishContent)
                    {
                        var price = _materialPrice.GetCurrentPrice(itm.materialid);
                        decimal itemprice = 0;
                        if (price == null)
                            itemprice = 0;
                        else
                            itemprice = price.price;

                        tdreplenish dreplenish = new tdreplenish()
                        {
                            amount = itm.replenishamount,
                            receiveamount = itm.replenishamount,
                            createdat = DateTime.Now,
                            createdby = Name,
                            id = itm.id,
                            price = itemprice,
                            rfidcode = itm.rfidCode,
                            replenishid = result.id,
                            materialid = itm.materialid
                        };
                        details.Add(dreplenish);
                    }
                    if (details.Count() != 0)
                    {
                        _replenishDetail.CreateMultiple(details.ToArray());
                    }
                }
                else
                {

                    var material = _material.GetAll();
                    var dta = _replenish.FindById(ID);
                    dta.updatedby = myGlobal.currentUser.nama;
                    _replenish.Update(dta);

                    var detail = (from rd in ReplenishContent
                                  select new tdreplenish()
                                  {
                                      amount = rd.amount,
                                      id = rd.id,
                                      materialid = rd.materialid,
                                      replenishid = ID,
                                      receiveamount = rd.replenishamount,
                                      rfidcode = rd.rfidCode
                                  }).ToArray();

                    var olddata = _replenishDetail.GetByReplenishId(ID).ToList();
                    var olddetail = (from d in olddata
                                     select new tdreplenish()
                                     {
                                         amount = d.amount,
                                         materialid = d.materialid,
                                         id = d.id,
                                         replenishid = d.replenishid
                                     }).ToArray();

                    _replenishDetail.UpdateMultiple(olddetail, detail);
                }
                MessageBox.Show("Data is Saved");
                IsSaved = true;
            }
            catch(Exception e)
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
                           replenishamount = d.receiveamount ?? 0,
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
