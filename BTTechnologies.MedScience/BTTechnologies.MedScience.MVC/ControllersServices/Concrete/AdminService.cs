using System;
using System.Collections.Generic;
using System.Web;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using System.Linq;
using BTTechnologies.MedScience.MVC.Helpers;
using BTTechnologies.MedScience.MVC.Infrastructure;
using BTTechnologies.MedScience.MVC.Infrastructure.Messages;
using BTTechnologies.MedScience.MVC.Infrastructure.Security;
using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Concrete
{
    public class AdminService : IAdminService
    {
        private readonly IAccountsRepository accountsRepository;
        private readonly IMembershipService membershipService;

        public AdminService(IAccountsRepository newAccountsRepository, IMembershipService newMembershipService)
        {
            accountsRepository = newAccountsRepository;
            membershipService = newMembershipService;
        }

        public BTTAjaxOutputGridModel GetUsersData(BTTAjaxInputGridModel inputGridModel)
        {
            const string idName = "Id";
            const string rolesName = "Roles";

            BTTAjaxOutputGridModel outputGridModel = inputGridModel.SortableColumnId == rolesName ?
                BTTAjaxGridHelper.GetOrderedGridData(GetOrderedDbSet(inputGridModel), inputGridModel) :
                BTTAjaxGridHelper.GetGridData(accountsRepository.Context.Accounts, inputGridModel);

            if (outputGridModel == null)
                return null;
            
            int roleId = 0;
            int identdierId = 0;
            int index = 0;
            foreach (string name in inputGridModel.ColumnNames)
            {
                if (name == idName)
                    identdierId = index;

                if (name == rolesName)
                    roleId = index;

                if (roleId != 0 && identdierId != 0)
                    break;

                index++;
            }

            foreach (string[] value in outputGridModel.ResultValues)
            {
                string idAsString = value[identdierId];
                int id;
                if (!int.TryParse(idAsString, out id))
                    continue;

                IList<string> names = accountsRepository.GetAccountRolesNames(id);
                value[roleId] = string.Join(", ", names);
            }

            return outputGridModel;
        }

        public ManageUserDataModel GetManageUserDataModel(int id)
        {
            Account account = accountsRepository.GetAccountById(id);
            if (account == null)
                return new ManageUserDataModel();

            ManageUserDataModel model = new ManageUserDataModel();
            model = ModelsMapper.CreateNewModelUsingMapping(model, account) ? model : new ManageUserDataModel();

            return model;
        }

        public object GetUserRolesData(int userId)
        {
            bool canManageAdmin = GetCanUserManageAdminPermissions();

            Account account = accountsRepository.GetAccountById(userId);
            if (account == null)
                return new { currentRoles = new string[0], currentRolesId = new int[0], rolesCanBeSelected = new string[0], rolesCanBeSelectedId = new int[0] };

            string[] currentRoles = new string[account.Roles.Count];
            int[] currentRolesId = new int[account.Roles.Count];
            int i = 0;
            foreach (Role role in account.Roles)
            {
                currentRoles[i] = role.DisplayName;
                currentRolesId[i] = role.Id;
                i++;
            }

            IList<string> adminRolesCodes = membershipService.GetAdminRolesCodes();
            IList<string> rolesCanBeSelected = new List<string>();
            IList<int> rolesCanBeSelectedId = new List<int>();

            i = 0;
            foreach (Role role in accountsRepository.GetRoles().Where(curRole => !account.Roles.Any(r => curRole.Id == r.Id)))
            {
                if (!canManageAdmin && adminRolesCodes.Contains(role.Code))
                    continue;

                rolesCanBeSelected.Add(role.DisplayName);
                rolesCanBeSelectedId.Add(role.Id);
                i++;
            }

            return new { currentRoles, currentRolesId, rolesCanBeSelected = rolesCanBeSelected.ToArray(), rolesCanBeSelectedId = rolesCanBeSelectedId.ToArray() };
        }

        public MedScienceErrors AddRoleToUser(int userId, string login, int permissionId)
        {
            bool canManageAdmin = GetCanUserManageAdminPermissions();
            if (!canManageAdmin && membershipService.UserIsAdmin(userId, false))
                return MedScienceErrors.NoAccessError;

            Account account = accountsRepository.GetAccountById(userId);
            Account userAccount = GetAccountByUserLogin(login);

            if (account == null || userAccount == null)
                return MedScienceErrors.UnknownError;

            if (userAccount.Id == account.Id)
                return MedScienceErrors.AdminCannotChangeHisRoles;

            accountsRepository.AddRoleWithId(account, permissionId);
            return MedScienceErrors.NoError;
        }

        public MedScienceErrors RemoveRoleFromUser(int userId, string login, int permissionId)
        {
            bool canManageAdmin = GetCanUserManageAdminPermissions();
            if (!canManageAdmin && membershipService.UserIsAdmin(userId, false))
                return MedScienceErrors.NoAccessError;

            Account account = accountsRepository.GetAccountById(userId);
            Account userAccount = GetAccountByUserLogin(login);

            if (account == null || userAccount == null)
                return MedScienceErrors.UnknownError;

            if (userAccount.Id == account.Id)
                return MedScienceErrors.AdminCannotChangeHisRoles;

            accountsRepository.RemoveRoleWithId(account, permissionId);
            return MedScienceErrors.NoError;
        }

        public MedScienceErrors RemoveUser(int userId, string login)
        {
            Account accountToDelete = accountsRepository.GetAccountById(userId);
            Account userAccount = GetAccountByUserLogin(login);

            if (accountToDelete == null || userAccount == null || !membershipService.UserHasPrivilege(login, Privilege.DeleteUserPrivilege))
                return MedScienceErrors.UnknownError;

            if (userAccount.Id == accountToDelete.Id)
                return MedScienceErrors.AdminCannotDeleteHimself;

            if (userAccount.Roles.All(r => r.Code != Role.SUPER_ADMIN_ROLE_CODE) && membershipService.UserIsAdmin(accountToDelete.Id, false))
                return MedScienceErrors.OnlySuperAdminCanDeleteOtherAdmins;

            try
            {
                accountsRepository.RemoveAccount(accountToDelete);
                return MedScienceErrors.NoError;
            }
            catch (Exception e)
            {
                ExceptionsLogger.LogException(e);
                return MedScienceErrors.UnknownError;
            }
        }

        public ManagePageTileModel GetManagePageTileModel(int pageTileId)
        {
            ManagePageTileModel model = new ManagePageTileModel();

            PageTile pageTile = accountsRepository.GetPageTileById(pageTileId);
            if (pageTile == null)
                return model;

            return ModelsMapper.CreateNewModelUsingMapping(model, pageTile) ? model : new ManagePageTileModel();
        }

        public MedScienceErrors AddEditPageTile(ManagePageTileModel model)
        {
            try
            {
                if (model == null)
                    return MedScienceErrors.UnknownError;

                PageTile pageTile = model.Id == 0 ? new PageTile() : accountsRepository.GetPageTileById(model.Id);

                if (pageTile == null || !ModelsMapper.CreateNewModelUsingMapping(pageTile, model))
                    return MedScienceErrors.UnknownError;

                accountsRepository.AddEditPageTile(pageTile);

                return MedScienceErrors.NoError;
            }
            catch (Exception e)
            {
                ExceptionsLogger.LogException(e);
                return MedScienceErrors.UnknownError;
            }
        }

        public BTTAjaxOutputGridModel GetTilesData(BTTAjaxInputGridModel inputGridModel)
        {
            return BTTAjaxGridHelper.GetGridData(accountsRepository.Context.PageTiles, inputGridModel);
        }

        public MedScienceErrors RemovePageTile(int rowId)
        {
            try
            {

                PageTile tile = accountsRepository.GetPageTileById(rowId);
                if (tile == null)
                    return MedScienceErrors.NoError;

                accountsRepository.RemovePageTile(tile);
                return MedScienceErrors.NoError;
            }
            catch (Exception e)
            {
                ExceptionsLogger.LogException(e);
                return MedScienceErrors.UnknownError;
            }
        }

        public Account GetAccountByUserLogin(string login)
        {
            return login == null ? null : accountsRepository.GetAccountByUserLogin(login.Trim());
        }

        private bool GetCanUserManageAdminPermissions()
        {
            string user = HttpContext.Current.User.Identity.Name;
            return membershipService.UserHasPrivilege(user, Privilege.ManageAdminPermissionsPrivilege);
        }

        private IOrderedQueryable<Account> GetOrderedDbSet(BTTAjaxInputGridModel inputGridModel)
        {
            return inputGridModel.AscentSort
                       ? accountsRepository.Context.Accounts.OrderBy(
                           acc => acc.Roles.Select(r => r.DisplayName).FirstOrDefault())
                       : accountsRepository.Context.Accounts.OrderByDescending(
                           acc => acc.Roles.Select(r => r.DisplayName).FirstOrDefault());
        }
    }
}