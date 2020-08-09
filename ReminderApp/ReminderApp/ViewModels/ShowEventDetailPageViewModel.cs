using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using ReminderApp.Services.SQLiteService;
using ReminderApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReminderApp.ViewModels
{
    public class ShowEventDetailPageViewModel : BaseViewModel
    {
        public ShowEventDetailPageViewModel(INavigationService navigationService = null, 
            IPageDialogService dialogService = null,
            ISqLiteService sqliteService = null) 
            : base(navigationService, dialogService, sqliteService)
        {
        }
    }
}
