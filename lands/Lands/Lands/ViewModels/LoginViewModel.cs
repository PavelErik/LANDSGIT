using System;
using System.Collections.Generic;
using System.Text;


namespace Lands.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Views;
    using System.ComponentModel;
    using System.Windows.Input;
    using Xamarin.Forms;
    using Services;

    public class LoginViewModel : BaseViewModel
    {
        #region Services
        private ApiService apiService;
        #endregion
        #region Attributes

        private string email;
        private string password;
        private bool isRunning;
        private bool isEnabled;
        #endregion
        #region Properties
        public string Email
        {
            get { return this.email; }
            set { SetValue(ref this.email, value); }
        }
        public string Password
        {
            get { return this.password; }
            set { SetValue(ref this.password, value); }
        }
        public bool IsRunning
        {
            get { return this.isRunning; }
            set { SetValue(ref this.isRunning, value); }
        }
        public bool IsRemembered { get; set; }
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { SetValue(ref this.isEnabled, value); }
        }

        #endregion
        #region Constructors
        public LoginViewModel()
        {
            this.apiService = new ApiService();
            this.IsRemembered = true;
            this.IsEnabled = true;
            //this.Email = "pavel.sist@gmail.com";
            //this.Password = "123";


        }
        #endregion
        #region Commands
        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand(Login);
            }

        }
        private async void Login()
        {
            if (string.IsNullOrEmpty(this.Email))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter and email",
                    "Accept");
                return;
            }
            if (string.IsNullOrEmpty(this.Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "You must enter and password",
                    "Accept");
                return;
            }





            this.IsRunning = true;
            this.IsEnabled = false;
            //if (this.Email != "pavel.sist@gmail.com" || this.Password != "123")
            //{
            //    this.IsRunning = false;
            //    this.IsEnabled = true;
            //    await Application.Current.MainPage.DisplayAlert(
            //        "Error",
            //        "Email o Password Incorrecto",
            //        "Accept"
            //        );
            //    this.Password = string.Empty;
            //    return;

            //}

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;

                await Application.Current.MainPage.DisplayAlert(
                  "Error",
                  connection.Message,
                  "Accept"); return;
            }

            var token = await this.apiService.GetToken("https://landsapi1810.azurewebsites.net", this.Email, this.Password);
            if (token == null)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                 "Error",
                 "Error al recuperar token, please try later",
                 "Accept"); return;
            }

            if (string.IsNullOrEmpty(token.AccessToken))
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                 "Error",
                 token.ErrorDescription,
                 "Accept");
                this.password = string.Empty;
                return;
            }

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Token = token;
            mainViewModel.Lands = new LandsViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new LandsPage());



            this.IsRunning = false;
            this.IsEnabled = true;

            this.Email = string.Empty;
            this.Password = string.Empty;

           

        }
        #endregion
    }


}
