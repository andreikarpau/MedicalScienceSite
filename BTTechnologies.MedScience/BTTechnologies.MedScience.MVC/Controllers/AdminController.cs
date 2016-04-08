using System.Collections.Generic;
using System.Web.Mvc;
using BTTechnologies.MedScience.MVC.App_LocalResources;
using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Infrastructure;
using BTTechnologies.MedScience.MVC.Infrastructure.Messages;
using BTTechnologies.MedScience.MVC.Infrastructure.Security;
using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.Controllers
{
    [Permission(Privilege.SeeAllUsersPrivilege)]
    public class AdminController : Controller
    {
        private readonly IAdminService adminService;

        public AdminController(IAdminService newAdminService)
        {
            adminService = newAdminService;
        }

        [Permission(Privilege.SeeAllUsersPrivilege)]
        public ActionResult UsersGrid()
        {
            AllUsersGridModel model = new AllUsersGridModel("AllUsersGrid", Url.Action("GetUsersData", "Admin"))
                                          {
                                              RowsPerPage = new List<int>{10, 20, 50}, 
                                              AddDeleteColumn = true, 
                                              AddEditColumn = true,
                                              EditRowUrl = Url.Action("ManageUser", "Admin"),
                                              DeleteRowUrl = Url.Action("DeleteUser", "Admin"),
                                              DeleteConfirmationText = MedSiteStrings.WantToDeleteRecordQuestion,
                                              DeleteTooltipText = MedSiteStrings.DeleteText,
                                              EditTooltipText = MedSiteStrings.EditText
                                          };

            model.Columns.Add(new BTTGridColumn { DisplayName = "", ColumnIdentifier = "Id", IsSortable = false, IsHidden = true, IsKey = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.UserEmailLogin, ColumnIdentifier = "UserLogin", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.UserName, ColumnIdentifier = "Name", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.Surname, ColumnIdentifier = "Surname", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.UserRoles, ColumnIdentifier = "Roles", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.RegistrationDate, ColumnIdentifier = "RegistrationDate", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.LastLogIn, ColumnIdentifier = "LastLogInDate", IsSortable = true });

            return View(model);
        }
        
        [HttpPost]
        [Permission(Privilege.ManageUserPrivilege)]
        public JsonResult ManageUser(int rowId)
        {
            return Json(new { manageUrl = Url.Action("ManageUserData", "Admin", new { id = rowId }) });
        }

        [HttpPost]
        [Permission(Privilege.DeleteUserPrivilege)]
        public JsonResult DeleteUser(int rowId)
        {
            MedScienceErrors error = adminService.RemoveUser(rowId, HttpContext.User.Identity.Name);
            return Json(error == MedScienceErrors.NoError ? string.Empty : EnumDescriptionHelper.GetEnumDescription(error));
        }

        [Permission(Privilege.ManageUserPrivilege)]
        public ActionResult ManageUserData(int id)
        {
            ManageUserDataModel model = adminService.GetManageUserDataModel(id);
            model.RefreshUrl = Url.Action("RefreshUserRoles");
            model.AddPermissionUrl = Url.Action("AddUserRole");
            model.DeletePermissionUrl = Url.Action("DeleteUserRole");

            return View(model);
        }
        
        [HttpPost]
        [Permission(Privilege.ManageAccountPermissionsPrivilege, Privilege.ManageAdminPermissionsPrivilege)]
        public JsonResult RefreshUserRoles(int userid)
        {
            return Json(adminService.GetUserRolesData(userid));
        }

        [HttpPost]
        [Permission(Privilege.ManageAccountPermissionsPrivilege, Privilege.ManageAdminPermissionsPrivilege)]
        public JsonResult AddUserRole(int userid, int permissionId)
        {
            MedScienceErrors error = adminService.AddRoleToUser(userid, HttpContext.User.Identity.Name, permissionId);
            return Json(error == MedScienceErrors.NoError ? string.Empty : EnumDescriptionHelper.GetEnumDescription(error));
        }

        [HttpPost]
        [Permission(Privilege.ManageAccountPermissionsPrivilege, Privilege.ManageAdminPermissionsPrivilege)]
        public JsonResult DeleteUserRole(int userid, int permissionId)
        {
            MedScienceErrors error = adminService.RemoveRoleFromUser(userid, HttpContext.User.Identity.Name, permissionId);
            return Json(error == MedScienceErrors.NoError ? string.Empty : EnumDescriptionHelper.GetEnumDescription(error));
        }

        [Permission(Privilege.SeeAllUsersPrivilege)]
        public JsonResult GetUsersData(BTTAjaxInputGridModel inputGridModel)
        {
            BTTAjaxOutputGridModel data = adminService.GetUsersData(inputGridModel);
            JsonResult result = Json(data);
            return result;
        }

        [Permission(Privilege.SeePageTiles)]
        public ActionResult TilesGrid()
        {
            BTTAjaxGridModel model = new BTTAjaxGridModel("TilesGrid", Url.Action("GetPageTilesData", "Admin"))
            {
                AddDeleteColumn = true,
                AddEditColumn = true,
                EditRowUrl = Url.Action("EditPageTile", "Admin"),
                DeleteRowUrl = Url.Action("DeletePageTile", "Admin"),
                DeleteConfirmationText = MedSiteStrings.WantToDeleteRecordQuestion,
                DeleteTooltipText = MedSiteStrings.DeleteText,
                EditTooltipText = MedSiteStrings.EditText
            };

            model.Columns.Add(new BTTGridColumn { DisplayName = "", ColumnIdentifier = "Id", IsSortable = false, IsHidden = true, IsKey = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.PageType, ColumnIdentifier = "PageType", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.TileType, ColumnIdentifier = "TileType", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.TyleStyles, ColumnIdentifier = "TileStyles", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.Order, ColumnIdentifier = "ItemOrder", IsSortable = true });

            return View(model);
        }

        [Permission(Privilege.AddPageTiles)]
        public ActionResult AddPageTile()
        {
            return View("ManagePageTile", new ManagePageTileModel());
        }

        [Permission(Privilege.ModifyPageTiles)]
        public ActionResult ManagePageTile(int id)
        {
            return View(adminService.GetManagePageTileModel(id));
        }

        [HttpPost]
        [Permission(Privilege.ModifyPageTiles, Privilege.AddPageTiles)]
        public ActionResult ManagePageTile(ManagePageTileModel model)
        {
            model.OperationSucceed = false;

            if (ModelState.IsValid)
            {
                MedScienceErrors error = adminService.AddEditPageTile(model);

                if (error == MedScienceErrors.NoError)
                    model.OperationSucceed = true;
                else
                    ModelState.AddModelError(string.Empty, EnumDescriptionHelper.GetEnumDescription(error));
            }

            return View(model);
        }

        [Permission(Privilege.SeePageTiles)]
        public JsonResult GetPageTilesData(BTTAjaxInputGridModel inputGridModel)
        {
            BTTAjaxOutputGridModel data = adminService.GetTilesData(inputGridModel);
            JsonResult result = Json(data);
            return result;
        }

        [HttpPost]
        [Permission(Privilege.ModifyPageTiles)]
        public JsonResult EditPageTile(int rowId)
        {
            return Json(new { manageUrl = Url.Action("ManagePageTile", "Admin", new { id = rowId }) });
        }

        [HttpPost]
        [Permission(Privilege.DeletePageTiles)]
        public JsonResult DeletePageTile(int rowId)
        {
            MedScienceErrors error = adminService.RemovePageTile(rowId);
            return Json(error == MedScienceErrors.NoError ? string.Empty : EnumDescriptionHelper.GetEnumDescription(error));
        }
    }
}
