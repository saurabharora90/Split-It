using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Split_It_.Model;
using Split_It_.Utils;
using System.Windows.Media;

namespace Split_It_
{
    public partial class ExpenseDetail : PhoneApplicationPage
    {
        Expense selectedExpense;

        public ExpenseDetail()
        {
            InitializeComponent();

            selectedExpense = PhoneApplicationService.Current.State[Constants.SELECTED_EXPENSE] as Expense;

            this.DataContext = selectedExpense;
            createAppBar();
        }

        private void createAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;
            ApplicationBar.BackgroundColor = (Color)Application.Current.Resources["green"];
            ApplicationBar.ForegroundColor = Colors.White;

            //delete expense button
            ApplicationBarIconButton btnDelete = new ApplicationBarIconButton();
            btnDelete.IconUri = new Uri("/Assets/Icons/delete.png", UriKind.Relative);
            btnDelete.Text = "delete";
            ApplicationBar.Buttons.Add(btnDelete);
            btnDelete.Click += new EventHandler(btnDelete_Click);

            //edit expense button
            ApplicationBarIconButton btnEdit = new ApplicationBarIconButton();
            btnEdit.IconUri = new Uri("/Assets/Icons/edit.png", UriKind.Relative);
            btnEdit.Text = "edit";
            ApplicationBar.Buttons.Add(btnEdit);
            btnEdit.Click += new EventHandler(btnEdit_Click);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

        }
    }
}