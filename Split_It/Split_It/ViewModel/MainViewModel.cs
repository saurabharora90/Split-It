using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Split_It.Model;
using Split_It.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        IDataService _dataService;
        INavigationService _navigationService;

        public MainViewModel(IDataService dataService, INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;

            initTasks();
        }

        private async void initTasks()
        {
            Task<User> userTask = _dataService.getCurrentUser();
            Task<IEnumerable<User>> friendsTask = _dataService.getFriendsList();
            //TODO: groups, recent activity
            await Task.WhenAll(userTask, friendsTask);

            CurrentUser = userTask.Result;
            FriendsList = new ObservableCollection<User>(friendsTask.Result);
        }

        #region Properties
        /// <summary>
        /// The <see cref="CurrentUser" /> property's name.
        /// </summary>
        public const string CurrentUserPropertyName = "CurrentUser";

        private User _currentUser = null;

        /// <summary>
        /// Sets and gets the CurrentUser property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return _currentUser;
            }

            set
            {
                if (_currentUser == value)
                {
                    return;
                }

                _currentUser = value;
                RaisePropertyChanged(CurrentUserPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="FriendsList" /> property's name.
        /// </summary>
        public const string FriendsListPropertyName = "FriendsList";

        private ObservableCollection<User> _friendsList = null;

        /// <summary>
        /// Sets and gets the FriendsList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<User> FriendsList
        {
            get
            {
                return _friendsList;
            }

            set
            {
                if (_friendsList == value)
                {
                    return;
                }

                _friendsList = value;
                RaisePropertyChanged(FriendsListPropertyName);
            }
        }
        #endregion
    }
}
