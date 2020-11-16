using digibox.services.Repositories.Interfaces;
using digibox.wind.Models;
using digibox.wind.Modules;
using digibox.wind.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace digibox.wind.ViewModel
{
    public class UserLoginViewModel:BaseViewModel
    {

        private IUserRepository _user;
        private UserModel _userModel;
        private UserControl _parent;

        public delegate void Login(Guid id);
        public delegate void Close(Object s);


        public UserControl Parent
        {
            get { return _parent; }
            set { _parent = value;
                OnPropertyChanged("Parent");
            }
        }

        public ICommand LoginCommand
        {
            get;
            private set;
        }

        public ICommand CancelCommand
        {
            get;
            private set;
        }

        private bool _islogged;
        public bool IsLogged
        {
            get
            {
                return _islogged;
            }
            set
            {
                _islogged = value;
                OnPropertyChanged("IsLogged");
            }
        }

        private bool _isError;

        public bool IsError
        {
            get { return _isError; }
            set { 
                _isError = value; 
                OnPropertyChanged("IsError");
            }
        }

        public event Login LoginEvent;
        public event Close CloseEvent;
        public UserLoginViewModel(IUserRepository user, UserModel userModel)
        {
            IsError = false;
            _user = user;
            _userModel = userModel;
            _islogged = false;
            LoginCommand = new RelayCommand((s) => LoginAction(s));
            CancelCommand = new RelayCommand((s) => CancelAction(s));
        }

        public string UserName
        {
            get { return _userModel.username; }
            set
            {
                IsError = false;
                _userModel.username = value;
                //ValidateProperty(value, "UserName");
                OnPropertyChanged("UserName");
            }
        }

        public string Password
        {
            get { return _userModel.password; }
            set
            {
                IsError = false;
                _userModel.password = value;
                OnPropertyChanged("Password");
            }
        }

        private void LoginAction(object s)
        {
            PasswordBox password = s as PasswordBox;

            var user = _user.Login(UserName, password.Password);

            IsLogged = user != null;
            if (_islogged)
            {
                var token = myGlobal.createToken();
                myGlobal.token = token;
                _user.updateUserToken(user.id, token);
                var currentUser = new UserModel()
                {
                    id = user.id,
                    nama = user.name,
                    roleid = user.roleid??Guid.Empty,
                    username = user.email
                };
                myGlobal.currentUser = currentUser;
                password.Password = "";
                LoginEvent(user.id);
            }
            IsError = true;
        }

        private void CancelAction(object s)
        {
            CloseEvent(s);
        }
    }
}
