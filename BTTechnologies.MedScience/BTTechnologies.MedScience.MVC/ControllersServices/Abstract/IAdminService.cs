using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.Infrastructure.Messages;
using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Abstract
{
    public interface IAdminService
    {
        BTTAjaxOutputGridModel GetUsersData(BTTAjaxInputGridModel inputGridModel);
        ManageUserDataModel GetManageUserDataModel(int id);
        object GetUserRolesData(int userId);
        MedScienceErrors AddRoleToUser(int userId, string login, int permissionId);
        MedScienceErrors RemoveRoleFromUser(int userId, string login, int permissionId);
        MedScienceErrors RemoveUser(int userId, string login);
        ManagePageTileModel GetManagePageTileModel(int pageTileId);
        MedScienceErrors AddEditPageTile(ManagePageTileModel model);
        BTTAjaxOutputGridModel GetTilesData(BTTAjaxInputGridModel inputGridModel);
        MedScienceErrors RemovePageTile(int rowId);
    }
}
