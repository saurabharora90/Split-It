using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Split_It.Model;
using Split_It.Model.Enum;
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
        ObservableCollection<Group> _allGroupsList;
        ObservableCollection<Friend> _allFriendsList;

        public ObservableCollection<GroupFilter> GroupsFiltersList { get; private set; }
        public ObservableCollection<FriendFilter> FriendsFiltersList { get; private set; }

        public MainViewModel(IDataService dataService, INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;

            FriendsFiltersList = new ObservableCollection<FriendFilter>();
            FriendsFiltersList.Add(FriendFilter.All);
            FriendsFiltersList.Add(FriendFilter.OwesYou);
            FriendsFiltersList.Add(FriendFilter.YouOwe);
            SelectedFriendFilter = FriendFilter.All;

            GroupsFiltersList = new ObservableCollection<GroupFilter>();
            GroupsFiltersList.Add(GroupFilter.RecentGroups);
            GroupsFiltersList.Add(GroupFilter.AllGroups);
            SelectedGroupFilter = GroupFilter.RecentGroups;

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

        #region Friends

        /// <summary>
        /// The <see cref="FriendsList" /> property's name.
        /// </summary>
        public const string FriendsListPropertyName = "FriendsList";

        /// <summary>
        /// Sets and gets the FriendsList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Friend> FriendsList
        {
            get
            {
                if(SelectedFriendFilter == FriendFilter.All)
                    return _allFriendsList;
                else
                {
                    ObservableCollection<Friend> filteredFriends = new ObservableCollection<Friend>();
                    foreach (var friend in _allFriendsList)
                    {
                        UserBalance defaultBalance = null;
                        foreach (var balance in friend.Balance)
                        {
                            if (defaultBalance == null)
                                defaultBalance = balance;

                            if (balance.CurrencyCode.Equals(CurrentUser.DefaultCurrency, StringComparison.CurrentCultureIgnoreCase) && Convert.ToDouble(balance.Amount) != 0)
                            {
                                defaultBalance = balance;
                                break;
                            }
                        }
                        if(defaultBalance!=null)
                        {
                            if (SelectedFriendFilter == FriendFilter.OwesYou && Convert.ToDouble(defaultBalance.Amount) > 0)
                                filteredFriends.Add(friend);
                            else if (SelectedFriendFilter == FriendFilter.YouOwe && Convert.ToDouble(defaultBalance.Amount) < 0)
                                filteredFriends.Add(friend);
                        }
                    }
                    return filteredFriends;
                }
            }
        }

        /// <summary>
        /// The <see cref="SelectedFriendFilter" /> property's name.
        /// </summary>
        public const string SelectedFriendFilterPropertyName = "SelectedFriendFilter";

        private FriendFilter _selectedFriendFilter;

        /// <summary>
        /// Sets and gets the SelectedFriendFilter property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public FriendFilter SelectedFriendFilter
        {
            get
            {
                return _selectedFriendFilter;
            }

            set
            {
                if (_selectedFriendFilter == value)
                {
                    return;
                }

                _selectedFriendFilter = value;
                RaisePropertyChanged(SelectedFriendFilterPropertyName);
                RaisePropertyChanged(FriendsListPropertyName);
            }
        }

        #endregion

        #region Groups

        /// <summary>
        /// The <see cref="GroupsList" /> property's name.
        /// </summary>
        public const string GroupsListPropertyName = "GroupsList";

        /// <summary>
        /// Sets and gets the GroupsList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Group> GroupsList
        {
            get
            {
                if (SelectedGroupFilter == GroupFilter.AllGroups)
                    return _allGroupsList;
                else
                {
                    ObservableCollection<Group> filteredGroupsList = new ObservableCollection<Group>();
                    foreach (var group in _allGroupsList)
                    {
                        bool canAdd = false;
                        Friend currentUser = null;
                        foreach (var member in group.Members)
                        {
                            if(member.id == CurrentUser.id)
                            {
                                currentUser = member;
                                break;
                            }
                        }

                        foreach (var balance in currentUser.Balance)
                        {
                            if(Convert.ToDouble(balance.Amount)!=0)
                            {
                                canAdd = true;
                                break;
                            }
                        }

                        if (canAdd || DateTime.Now.Subtract(Convert.ToDateTime(group.UpdatedAt)).Days < 30)
                            filteredGroupsList.Add(group);
                    }
                    return filteredGroupsList;
                }
            }
        }

        /// <summary>
        /// The <see cref="SelectedGroupFilter" /> property's name.
        /// </summary>
        public const string SelectedGroupFilterPropertyName = "SelectedGroupFilter";

        private GroupFilter _selectedGroupFilter;

        /// <summary>
        /// Sets and gets the SelectedGroupFilter property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public GroupFilter SelectedGroupFilter
        {
            get
            {
                return _selectedGroupFilter;
            }

            set
            {
                if (_selectedGroupFilter == value)
                {
                    return;
                }

                _selectedGroupFilter = value;
                RaisePropertyChanged(SelectedGroupFilterPropertyName);
                RaisePropertyChanged(GroupsListPropertyName);
            }
        }

        #endregion

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

                        _allFriendsList = new ObservableCollection<Friend>(friendsTask.Result);
                        _allGroupsList = new ObservableCollection<Group>(groupsTask.Result);

                        RaisePropertyChanged(FriendsListPropertyName);
                        RaisePropertyChanged(GroupsListPropertyName);

                        IsBusy = false;
                    }));
            }
        }
        #endregion
    }
}
