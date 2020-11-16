using Autofac;
using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.Interfaces;
using digibox.services.Services.interfaces;
using digibox.wind.Models;
using digibox.wind.Modules;
using digibox.wind.View;
using digibox.wind.ViewModel.Base;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZXing.QrCode.Internal;

namespace digibox.wind.ViewModel
{
    public class ReplenishListViewModel:BaseViewModel
    {
        private readonly IRoleService _role;
        private readonly IReplenishRepository _replenish;
        private readonly IUserRepository _user;
        private readonly IMaterialRepository _material;
        private readonly IReplenishDetailRepository _replenishDetail;

        //menampilkan daftar replenish.
        public ReplenishListViewModel(IReplenishRepository replenish, IReplenishDetailRepository replenishDetail, IMaterialRepository material, IUserRepository user, IRoleService role)
        {
            _role = role;
            _replenish = replenish;
            _user = user;
            _material = material;
            _replenishDetail = replenishDetail;
            SearchCommand = new RelayCommand((x) => Search(x));
            OpenData();
        }

        private ObservableCollection<ReplenishListModel> _replenishlist;

        public ObservableCollection<ReplenishListModel> ReplenishList
        {
            get { return _replenishlist; }
            set
            {
                _replenishlist = value;
                OnPropertyChanged("ReplenishList");
            }
        }

        private MainViewModel _caller;

        public MainViewModel Caller
        {
            get { return _caller; }
            set { _caller = value; }
        }

        private void OpenData(string param = "")
        {
            var user = myGlobal.currentUser;
            var role = _role.getRoleByToken(myGlobal.token);
            if (role == userRole.COLLECTOR)
            {
                OpenDataCollector(param);
            }
            if (role == userRole.ADMIN)
            {
                Console.WriteLine("ADMIN REPLENISH LIST");
            }
        }

        private void OpenDataCollector(String param = "" )
        {
            var user = myGlobal.currentUser;
            var mdl = _replenish.GetByCollector(user.id).ToList();
            var users = _user.GetAll().ToList();
            var model = (from o in mdl
                         join u in users on o.receivedbyid equals u.id into usr
                         from us in usr.DefaultIfEmpty()
                         select new ReplenishListModel()
                         {
                             id = o.id,
                             indate = o.indate,
                             no = o.no,
                             status = o.status,
                             receiverName = us == null ? "" : us.name,
                             receiveddate = o.receiveddate,
                             isDeletable = o.status==ReplenishStatus.DRAFT
                         }).ToList();

            var material = _material.GetAll();
            foreach (var itm in model)
            {
                var mats = _replenishDetail.GetByReplenishId(itm.id).ToList();
                var dtam = (from m in mats
                            join mt in material on m.materialid equals mt.id
                            select mt.name).ToArray();
                var smats = String.Join(", ", dtam);
                itm.editDataEvent += Itm_editDataEvent;

                itm.items= smats;
            }

            model = model.Where(x => x.items.Contains(param)).ToList();

            foreach (var itm in model)
            {
                itm.editDataEvent += Itm_editDataEvent;
                itm.removeDataEvent += Itm_removeDataEvent;
                itm.confirmDataEvent += Itm_confirmDataEvent;
            }
            ReplenishList = new ObservableCollection<ReplenishListModel>(model);
        }

        private void Search(object x)
        {
            OpenData(SearchParam);
        }

        public ICommand SearchCommand
        {
            get;
            private set;
        }

        private string _searchParam;

        public string SearchParam
        {
            get { return _searchParam; }
            set
            {
                _searchParam = value;
                OnPropertyChanged("SearchParam");
            }
        }


        private void Itm_confirmDataEvent(Guid id)
        {
            var repl = _replenish.FindById(id);
            var container = iocInit.ConfigureContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                var _replenishCornfimViewModel = scope.Resolve<ReplenishConfirmViewModel>();
                var _replenishconfirmview = scope.Resolve<ReplenishConfirmView>();
                _replenishconfirmview.DataContext = _replenishCornfimViewModel;

                var rep = _replenish.FindById(id);
                var usr = _user.FindById(rep.collectorid ?? Guid.Empty);
                _replenishCornfimViewModel.UserID = rep.id;
                _replenishCornfimViewModel.ID = id;
                _replenishCornfimViewModel.CurrentDate = rep.indate ?? DateTime.Now;
                _replenishCornfimViewModel.Name = usr.name;
                _replenishCornfimViewModel.isEdit = true;
                _replenishCornfimViewModel.LoadDetail();
                Caller.CurrentViewModel = _replenishCornfimViewModel;
                Caller.CurrentView = _replenishconfirmview;
            }
        }


        //This id is belong to id repleish
        private void Itm_removeDataEvent(Guid id)
        {
            if (MessageBox.Show("You about to delete a record. Are you sure?", "Delete Data", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            ttreplenish replenish = new ttreplenish()
            {
                id = id,
                deletedby = myGlobal.currentUser.nama
            };

            _replenish.Delete(replenish);
            MessageBox.Show("Data is Deleted");
            var removedata = ReplenishList.Where(x => x.id == id).FirstOrDefault();
            ReplenishList.Remove(removedata);
        }

        private void Itm_editDataEvent(Guid id)
        {
            var repl = _replenish.FindById(id);
            var container = iocInit.ConfigureContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                var _replenishListViewModel = scope.Resolve<ReplenishMaterialViewModel>();
                var _replenishlistview = scope.Resolve<ReplenishMaterialView>();
                _replenishlistview.DataContext = _replenishListViewModel;

                var rep = _replenish.FindById(id);
                var usr = _user.FindById(rep.collectorid??Guid.Empty);
                _replenishListViewModel.UserID = rep.id;
                _replenishListViewModel.ID = id;
                _replenishListViewModel.CurrentDate = rep.indate??DateTime.Now;
                _replenishListViewModel.Name = usr.name;
                _replenishListViewModel.isEdit = true;
                _replenishListViewModel.LoadDetail();
                Caller.CurrentViewModel = _replenishListViewModel;
                Caller.CurrentView = _replenishlistview;
            }
        }
    }
}
