using Autofac;
using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories;
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace digibox.wind.ViewModel
{
    public class DrawListViewModel:BaseViewModel
    {
        private readonly IRoleService _role;
        private readonly IDrawRepository _draw;
        private readonly IUserRepository _user;
        private readonly IDrawDetailRepository _drawDetail;
        private readonly IMaterialRepository _material;
        

        //menampilkan daftar replenish.
        public DrawListViewModel(IDrawRepository draw,  IDrawDetailRepository drawDetail, IMaterialRepository material, IUserRepository user, IRoleService role)
        {
            _role = role;
            _draw = draw;
            _user = user;
            _drawDetail = drawDetail;
            _material = material;
            SearchCommand = new RelayCommand((x) => Search(x));

            OpenData();
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
            set { _searchParam = value;
                OnPropertyChanged("SearchParam");
            }
        }


        private ObservableCollection<DrawListModel> _drawlist;

        public ObservableCollection<DrawListModel> DrawList
        {
            get { return _drawlist; }
            set
            {
                _drawlist = value;
                OnPropertyChanged("DrawList");
            }
        }

        private void OpenData(string param="")
        {
            var user = myGlobal.currentUser;
            var role = _role.getRoleByToken(myGlobal.token);
            if (role == userRole.USER)
            {
                OpenDataUser(user.id,param);
            }
            if (role == userRole.ADMIN)
            {
                Console.WriteLine("ADMIN REPLENISH LIST");
            }
        }

        private void OpenDataUser(Guid id, string param="")
        {
            var dta = _draw.GetByDrawer(id).ToList();

            var users = _user.GetAll().ToList();
            var model = (from o in dta
                         select new DrawListModel()
                         {
                             id = o.id,
                             drawdate = o.drawdate,
                             no = o.no,
                             status = o.status,
                             receiveddate = o.receiveddate,
                             isDeletable = o.status == ReplenishStatus.DRAFT
                         }).ToList();

            var material = _material.GetAll();
            foreach (var itm in model)
            {
                var mats = _drawDetail.GetByDrawId(itm.id).ToList();
                var dtam = (from m in mats
                            join mt in material on m.materialid equals mt.id
                            select mt.name).ToArray();
                var smats = String.Join(", ", dtam);
                itm.editDataEvent += Itm_editDataEvent;

                itm.Items = smats;
            }
            model = model.Where(x => x.Items.Contains(param)).ToList();
            DrawList = new ObservableCollection<DrawListModel>(model);

        }

        private MainViewModel _caller;

        public MainViewModel Caller
        {
            get { return _caller; }
            set { _caller = value; }
        }

        //This id is belong to id repleish
        private void Itm_editDataEvent(Guid id)
        {
            var repl = _draw.FindById(id);
            var container = iocInit.ConfigureContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                var _drawListViewModel = scope.Resolve<DrawMaterialViewModel>();
                var _replenishlistview = scope.Resolve<DrawMaterialView>();
                _replenishlistview.DataContext = _drawListViewModel;

                var rep = _draw.FindById(id);
                var usr = _user.FindById(rep.drawerid??Guid.Empty);
                _drawListViewModel.UserID = rep.id;
                _drawListViewModel.ID = id;
                _drawListViewModel.CurrentDate = rep.drawdate??DateTime.Now;
                _drawListViewModel.Name = usr.name;
                _drawListViewModel.isEdit = false;
                _drawListViewModel.LoadDetail();
                Caller.CurrentViewModel = _drawListViewModel;
                Caller.CurrentView = _replenishlistview;
            }
        }
    }
}
