using digibox.data;
using digibox.services.Repositories.Interfaces;
using digibox.services.Services.interfaces;
using digibox.wind.Models;
using digibox.wind.Modules;
using digibox.wind.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace digibox.wind.ViewModel
{
    public class DrawMaterialViewModel:BaseViewModel
    {
        private readonly IDrawRepository _draw;
        private readonly IDrawDetailRepository _drawDetail;
        private readonly IMaterialPriceRepository _materialPrice;
        private readonly IInventoryRepository _inventory;
        private readonly IUserRepository _user;
        private readonly IMaterialRepository _material;
        private readonly IMaterialServices _materialServices;
        public delegate void Done();


        public event Done DoneEvent;

        public DrawMaterialViewModel(IDrawRepository draw, IDrawDetailRepository drawDetail, IInventoryRepository inventory,
            IMaterialRepository material, IMaterialServices materialServices, IMaterialPriceRepository materialPrice,
            IUserRepository user)
        {
            _user = user;
            _material = material;
            _materialServices = materialServices;
            _draw = draw;
            _drawDetail = drawDetail;
            _materialPrice = materialPrice;
            _inventory = inventory;

            //initialize property
            RFIDCode = "";

            var currentuser = myGlobal.currentUser;
            UserID = currentuser.id;
            Name = currentuser.nama;
            CurrentDate = DateTime.Now;

            DrawContent = new ObservableCollection<DrawMaterialListModel>();
            //initialize command
            AddItemCommand = new RelayCommand((x) => AddItem(x), (y) => CanAddItem(y));
            ConfirmCommand = new RelayCommand((x) => Confirm(x), (y) => CanConfirmData(y));
        }

        private void Confirm(object x)
        {

            if(MessageBox.Show("You are about to confirm material draw. Are you Sure?","Draw Material", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            //here will be finish by tomorrow.
            //saving to 
            var user = myGlobal.currentUser;
            ttdraw draw = new ttdraw()
            {
                id = Guid.NewGuid(),
                no = _no,
                drawdate = CurrentDate,
                drawerid = user.id,
                createdby = user.nama,
                status = ReplenishStatus.RECEIVED,
                isdeleted = false
            };

            List<tddraw> details = new List<tddraw>();
            //detail;
            foreach (var itm in DrawContent)
            {
                var price = _materialPrice.GetCurrentPrice(itm.materialid);
                decimal itemprice = 0;
                if (price == null)
                    itemprice = 0;
                else
                    itemprice = price.price;

                tddraw ddraw = new tddraw()
                {
                    amount = itm.amount,
                    receiveamount = itm.DrawAmount,
                    createdat = DateTime.Now,
                    createdby = Name,
                    id = Guid.NewGuid(),
                    price = itemprice,
                    rfidcode = itm.rfidCode,
                    drawid= draw.id,
                    materialid = itm.materialid,
                    isdeleted =false
                };
                details.Add(ddraw);
            }

            _draw.Received(draw, details);
            DoneEvent();
            //move to main.
        }

        private bool CanConfirmData(object y)
        {
            return DrawContent.Count != 0;
        }

        private string _RFIDCode;
        public string RFIDCode
        {
            get { return _RFIDCode; }
            set
            {
                _RFIDCode = value;
                OnPropertyChanged("RFIDCode");
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _no;

        public string No
        {
            get { return _no; }
            set { _no = value; 
                OnPropertyChanged("No");
            }
        }


        private DateTime _currentDate;
        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                _currentDate = value;
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
            set
            {
                _userID = value;
                OnPropertyChanged("ID");
            }
        }

        public bool isEdit { get; set; }

        private ObservableCollection<DrawMaterialListModel> _drawContent;

        public ObservableCollection<DrawMaterialListModel> DrawContent
        {
            get
            {
                return _drawContent;
            }
            set
            {
                _drawContent = value;
                OnPropertyChanged("DrawContent");
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

            //kalau data udah ada sebelumnya.
            var inList = DrawContent.Where(x => x.rfidCode == RFIDCode);
            if (inList.Count() != 0)
            {
                //nambah data 
                var item = inList.FirstOrDefault();
                item.DrawAmount = item.DrawAmount + 1;
                return;
            }

            //check inventory
            var iv = _inventory.GetByRFIDCode(RFIDCode).FirstOrDefault();
            if (iv == null)
            {
                MessageBox.Show("Wrong RFID Code!");
                return;
            }
            var material = _material.FindById(iv.materialid);
            //Adding to list in the 
            var materialinfo = _materialServices.GetMaterialCurrentPrice().Where(x => x.id == iv.materialid).FirstOrDefault();

            CultureInfo CI = new CultureInfo("id-ID");
            Calendar Cal = CI.Calendar;
            DateTime currentdate = DateTime.Now;
            int week = Cal.GetWeekOfYear(currentdate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            string precode = string.Format("{0:yyyy}{1}", currentdate, week);

            DrawMaterialListModel content = new DrawMaterialListModel()
            {
                materialid = iv.materialid,
                materialname = material.name,
                maximum = materialinfo.maxstock ?? 0,
                amount = materialinfo.currentstock ?? 0,
                partno = material.partno,
                unit = material.unit,
                drawamount = 1,
                rfidCode = RFIDCode,
            };

            content.removeDataEvent += Itm_removeDataEvent;
            DrawContent.Add(content);
            RFIDCode = String.Empty;
        }


        public void LoadDetail()
        {
            var drawd = _drawDetail.GetByDrawId(ID).ToList();
            var materialinfo = _materialServices.GetMaterialCurrentPrice().ToList();
            var dta = (from d in drawd
                       join m in materialinfo on d.materialid equals m.id
                       select new DrawMaterialListModel()
                       {
                           materialid = d.id,
                           drawamount = d.receiveamount ?? 0,
                           amount = d.amount,
                           materialname = m.name,
                           maximum = m.maxstock ?? 0,
                           partno = m.partno,
                           rfidCode = d.rfidcode,
                           unit = m.unit
                       }).ToList();
            foreach (var itm in dta)
            {
                itm.removeDataEvent += Itm_removeDataEvent;
            }
            DrawContent = new ObservableCollection<DrawMaterialListModel>(dta);
        }

        private void Itm_removeDataEvent(Guid id)
        {
            var dta = DrawContent.Where(x => x.id == id).FirstOrDefault();
            DrawContent.Remove(dta);
        }
    }
}
