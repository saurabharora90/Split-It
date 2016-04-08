using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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

            getCurrentUSer();
            RefreshDataCommand.Execute(null);
        }

        private async void getCurrentUSer()
        {
            CurrentUser = await _dataService.getCurrentUser();
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

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy = true;

        /// <summary>
        /// Sets and gets the IsBusy property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                if (_isBusy == value)
                {
                    return;
                }

                _isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);
            }
        }
        #endregion

        #region Commands
        private RelayCommand _refreshDataCommand;

        /// <summary>
        /// Gets the RefreshDataCommand.
        /// </summary>
        public RelayCommand RefreshDataCommand
        {
            get
            {
                return _refreshDataCommand
                    ?? (_refreshDataCommand = new RelayCommand(
                    async () =>
                    {
                        IsBusy = true;
                        Task<IEnumerable<Friend>> friendsTask = _dataService.getFriendsList();
                        Task<IEnumerable<Group>> groupsTask = _dataService.getGroupsList();
                        //TODO: recent activity
                        await Task.WhenAll(friendsTask, groupsTask);

                        FriendsList = new ObservableCollection<Friend>(friendsTask.Result);
                        GroupsList = new ObservableCollection<Group>(groupsTask.Result);
                        IsBusy = false;
                    }));
            }
        }
        #endregion
    }
}
