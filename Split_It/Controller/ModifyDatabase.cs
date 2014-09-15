﻿using Microsoft.Phone.Shell;
using Split_It_.Model;
using Split_It_.Request;
using Split_It_.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Controller
{
    public class ModifyDatabase
    {
        Action<bool, HttpStatusCode> callback;

        public ModifyDatabase(Action<bool, HttpStatusCode> callback)
        {
            this.callback = callback;
        }

        public void deleteExpense(int expenseId)
        {
            DeleteExpenseRequest request = new DeleteExpenseRequest(expenseId);
            request.deleteExpense(_OperationSucceded, _OperationFailed);
        }

        public void editExpense(Expense editedExpenseDetail)
        {
            UpdateExpenseRequest request = new UpdateExpenseRequest(editedExpenseDetail);
            request.updateExpense(_OperationSucceded, _OperationFailed);
        }

        public void addExpense(Expense expense)
        {
            AddExpenseRequest request = new AddExpenseRequest(expense);
            request.addExpense(_OperationSucceded, _OperationFailed);
        }

        public void createFriend(string email, string firstName, string lastName)
        {
            CreateFriendRequest request = new CreateFriendRequest(email, firstName, lastName);
            request.createFriend(_FriendAdded, _OperationFailed);
        }

        public void createGroup(Group group)
        {
            CreateGroupRequest request = new CreateGroupRequest(group);
            request.createGroup(_GroupAdded, _OperationFailed);
        }

        private void _FriendAdded(User friend)
        {
            //add user to database and to friends list in App.xaml
            PhoneApplicationService.Current.State[Constants.NEW_USER] = friend;
            
            SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true);
            dbConn.Insert(friend);
            friend.picture.user_id = friend.id;
            dbConn.Insert(friend.picture);
            dbConn.Close();

            callback(true, HttpStatusCode.OK);
        }

        private void _GroupAdded(Group group)
        {
            //add user to database and to friends list in App.xaml
            PhoneApplicationService.Current.State[Constants.NEW_GROUP] = group;

            SQLiteConnection dbConn = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite, true);
            dbConn.BeginTransaction();
            dbConn.Insert(group);
            
            foreach (var debt in group.simplified_debts)
            {
                debt.group_id = group.id;
                dbConn.InsertOrReplace(debt);
            }
            
            foreach (var member in group.members)
            {
                Group_Members group_member = new Group_Members();
                group_member.group_id = group.id;
                group_member.user_id = member.id;
                dbConn.InsertOrReplace(group_member);
            }
            
            dbConn.Commit();
            dbConn.Close();

            callback(true, HttpStatusCode.OK);
        }
        
        
        private void _OperationSucceded(bool status)
        {
            callback(true, HttpStatusCode.OK);
        }

        private void _OperationFailed(HttpStatusCode statusCode)
        {
            callback(false, statusCode);
        }
    }
}
