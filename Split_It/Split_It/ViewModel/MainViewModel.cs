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
            Task<IEnumerable<Friend>> friendsTask = _dataService.getFriendsList();
            Task<IEnumerable<Group>> groupsTask = _dataService.getGroupsList();
            //TODO: recent activity
            await Task.WhenAll(userTask, friendsTask, groupsTask);

            CurrentUser = userTask.Result;
            FriendsList = new ObservableCollection<Friend>(friendsTask.Result);
            GroupsList = new ObservableCollection<Group>(groupsTask.Result);
        }

        #region Properties
        /// <summary>
        /// The <see cref="CurrentUser" /> property's name.
        /// </summary>
        public const string CurrentUserPropertyName = "CurrentUser";

        private User _currentUser = Utils.Util.getCurrentUserData();

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
                Utils.Util.saveCurrentUserData(value);
            }
        }

        /// <summary>
        /// The <see cref="FriendsList" /> property's name.
        /// </summary>
        public const string FriendsListPropertyName = "FriendsList";

        private ObservableCollection<Friend> _friendsList = null;

        /// <summary>
        /// Sets and gets the FriendsList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Friend> FriendsList
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

        /// <summary>
        /// The <see cref="GroupsList" /> property's name.
        /// </summary>
        public const string GroupsListPropertyName = "GroupsList";

        private ObservableCollection<Group> _groupsList = null;

        /// <summary>
        /// Sets and gets the GroupsList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Group> GroupsList
        {
            get
            {
                return _groupsList;
            }

            set
            {
                if (_groupsList == value)
                {
                    return;
                }

                _groupsList = value;
                RaisePropertyChanged(GroupsListPropertyName);
            }
        }
        #endregion
    }
}
