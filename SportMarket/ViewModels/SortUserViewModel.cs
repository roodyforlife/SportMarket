using SportMarket.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class SortUserViewModel
    {
        public UsersSortState NameSort { get; private set; }
        public UsersSortState BirthdaySort { get; private set; }
        public UsersSortState SexSort { get; private set; }
        public UsersSortState BonusesSort { get; private set; }
        public UsersSortState TwoFactorSort { get; private set; }
        public UsersSortState SurnameSort { get; private set; }
        public UsersSortState RegistDateSort { get; private set; }
        public UsersSortState EmailSort { get; private set; }
        public UsersSortState Current { get; private set; }

        public SortUserViewModel(UsersSortState sortOrder)
        {
            NameSort = sortOrder == UsersSortState.NameAsc ? UsersSortState.NameDesc : UsersSortState.NameAsc;
            BirthdaySort = sortOrder == UsersSortState.BirthdayAsc ? UsersSortState.BirthdayDesc : UsersSortState.BirthdayAsc;
            SexSort = sortOrder == UsersSortState.SexAsc ? UsersSortState.SexDesc : UsersSortState.SexAsc;
            BonusesSort = sortOrder == UsersSortState.BonusesAsc ? UsersSortState.BonusesDesc : UsersSortState.BonusesAsc;
            TwoFactorSort = sortOrder == UsersSortState.TwoFactorAsc ? UsersSortState.TwoFactorDesc : UsersSortState.TwoFactorAsc;
            SurnameSort = sortOrder == UsersSortState.SurnameAsc ? UsersSortState.SurnameDesc: UsersSortState.SurnameAsc;
            RegistDateSort = sortOrder == UsersSortState.RegisDateAsc ? UsersSortState.RegistDateDesc : UsersSortState.RegisDateAsc;
            EmailSort = sortOrder == UsersSortState.EmailAsc ? UsersSortState.EmailDesc : UsersSortState.EmailAsc;
            Current = sortOrder;
        }
    }
}
