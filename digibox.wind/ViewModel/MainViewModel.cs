using Autofac;
using digibox.wind.Modules;
using digibox.wind.View;
using digibox.wind.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace digibox.wind.ViewModel
{
    public class MainViewModel: BaseViewModel
    {
				private BaseViewModel _currentViewModel;

				public BaseViewModel CurrentViewModel
				{
						get { return _currentViewModel; }
						set
						{
								_currentViewModel = value;
								OnPropertyChanged("CurrentViewModel");
						}
				}

				private UserControl _currentView;
				public UserControl CurrentView
				{
						get { return _currentView; }
						set
						{
								_currentView = value;
								OnPropertyChanged("CurrentView");
						}
				}

				private string _userName;

				public string UserName
				{
						get { return _userName; }
						set { _userName = value;
								OnPropertyChanged("UserName");
						}
				}


				public ICommand ReplenishCommand
				{
						get;
						private set;
				}
				public ICommand CreateCommand
				{
						get;
						private set;
				}

				public ICommand DrawMaterialCommand
				{
						get;
						private set;
				}
				public ICommand DrawCommand
				{
						get;
						private set;
				}

				public ICommand LogoutCommand
				{
						get;
						private set;
				}


				private bool _isLogin;

				public bool IsLogin
				{
						get { return _isLogin; }
						set { _isLogin = value;
								OnPropertyChanged("IsLogin");
						}
				}


				private UserLoginViewModel _userLoginModel;

				public UserLoginViewModel UserLoginModel
				{
						get { return _userLoginModel; }
						set { _userLoginModel = value;
								OnPropertyChanged("UserLoginModel");
						}
				}

				private bool _isError;

				public bool IsError
				{
						get { return _isError; }
						set
						{
								_isError = value;
								OnPropertyChanged("IsError");
						}
				}

				public MainViewModel()
				{
						ReplenishCommand = new RelayCommand((s) => ReplenishList(s));
						CreateCommand = new RelayCommand((s) => CreateReplenish(s));
						DrawMaterialCommand = new RelayCommand((s) => CreateDraw(s));
						DrawCommand = new RelayCommand((s) => DrawList(s));
						LogoutCommand = new RelayCommand((s) => Logout(s));
						IsLogin = false;
						IsError = false;
						InitialLogin();
				}

				private void Logout(object s)
				{
						IsLogin = false;
				}

				private void InitialLogin()
				{
						var container = iocInit.ConfigureContainer();
						using (var scope = container.BeginLifetimeScope())
						{
								_userLoginModel = scope.Resolve<UserLoginViewModel>();
								_userLoginModel.LoginEvent += _userLoginModel_LoginEvent;
								_userLoginModel.CloseEvent += _userLoginModel_CloseEvent;	
						}

				}

				private void _userLoginModel_CloseEvent(object s)
				{
						Environment.Exit(0);
				}

				private void _userLoginModel_LoginEvent(Guid id)
				{
						UserName = myGlobal.currentUser.nama;
						IsLogin = true;
						Home();
				}

				private void Home()
				{
						var container = iocInit.ConfigureContainer();
						using (var scope = container.BeginLifetimeScope())
						{
							//	var _drawListViewModel = scope.Resolve<DrawListViewModel>();
								var _homeview = scope.Resolve<HomeView>();
								//_drawview.DataContext = _drawListViewModel;
								//_drawListViewModel.Caller = this;
								CurrentView = _homeview;
								//CurrentViewModel = _drawListViewModel;
						}
				}

				private void DrawList(object s)
				{
						var container = iocInit.ConfigureContainer();
						using (var scope = container.BeginLifetimeScope())
						{
								var _drawListViewModel = scope.Resolve<DrawListViewModel>();
								var _drawview = scope.Resolve<DrawView>();
								_drawview.DataContext = _drawListViewModel;
								_drawListViewModel.Caller = this;
								CurrentView = _drawview;
								CurrentViewModel = _drawListViewModel;
						}
				}

				private void CreateDraw(object s)
				{
						var container = iocInit.ConfigureContainer();
						using (var scope = container.BeginLifetimeScope())
						{
								var _materialViewModel = scope.Resolve<DrawMaterialViewModel>();
								var _materialview = scope.Resolve<DrawMaterialView>();
								_materialview.DataContext = _materialViewModel;
								_materialViewModel.DoneEvent += _materialViewModel_DoneEvent;
								CurrentView = _materialview;
								CurrentViewModel = _materialViewModel;

						}
				}

				private void _materialViewModel_DoneEvent()
				{
						var container = iocInit.ConfigureContainer();
						using (var scope = container.BeginLifetimeScope())
						{
								var _drawListViewModel = scope.Resolve<DrawListViewModel>();
								var _drawview = scope.Resolve<DrawView>();
								_drawview.DataContext = _drawListViewModel;
								_drawListViewModel.Caller = this;
								CurrentView = _drawview;
								CurrentViewModel = _drawListViewModel;
						}
				}

				private void ReplenishList(object s)
				{
						var container = iocInit.ConfigureContainer();
						using (var scope = container.BeginLifetimeScope())
						{
								var _replenishListViewModel = scope.Resolve<ReplenishListViewModel>();
								var _replenishlistview = scope.Resolve<ReplenishListView>();
								_replenishlistview.DataContext = _replenishListViewModel;
								_replenishListViewModel.Caller = this;
								CurrentView = _replenishlistview;
								CurrentViewModel = _replenishListViewModel;
						}
				}

				private void CreateReplenish(object s)
				{
					var container = iocInit.ConfigureContainer();
						using (var scope = container.BeginLifetimeScope())
						{
								var _replenishListViewModel = scope.Resolve<ReplenishMaterialViewModel>();
								var _replenishlistview = scope.Resolve<ReplenishMaterialView>();
								_replenishlistview.DataContext = _replenishListViewModel;
								CurrentView = _replenishlistview;
								CurrentViewModel = _replenishListViewModel;
						}
				}
		}
}
