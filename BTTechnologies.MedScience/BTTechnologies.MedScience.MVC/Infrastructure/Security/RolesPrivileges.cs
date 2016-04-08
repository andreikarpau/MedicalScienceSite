using System.Collections.Generic;
using BTTechnologies.MedScience.Domain.Entities;

namespace BTTechnologies.MedScience.MVC.Infrastructure.Security
{
    public enum Privilege
    {
        EmptyPrivilege,

        // Account
        ActivateYourselfPrivilege,

        // Admin
        SeeAllUsersPrivilege,
        ManageUserPrivilege,
        DeleteUserPrivilege,
        ManageAccountPermissionsPrivilege,
        ManageAdminPermissionsPrivilege,

        AddPageTiles,
        ModifyPageTiles,
        DeletePageTiles,
        SeePageTiles,

        // Author
        SeeAllAuthorsPrivilege,
        AddAuthorsPrivilege,
        ManageAuthorsPrivilege, 
        DeleteAuthorPrivilege,
        ManageAuthorYourselfPrivilege,
        EditAuthorAccountPrivilege,

        // Documents
        SeeAllDocumentsPrivilege,
        AddDocumentPrivilege,
        ManageDocumentPrivilege,
        DeleteDocumentPrivilege,
        AddCategoryPrivilege,
        CanPublishArticle,
        SeeSiteFiles,
        AddSiteFiles,
        DeleteSiteFiles,

        //Logs
        SeeArticleLogPrivilege
    }

    public static class RolesPrivileges
    {
        public static Privilege[] NonActiveUserPrivileges = { Privilege.EmptyPrivilege, Privilege.ActivateYourselfPrivilege };

        public static Privilege[] UserPrivileges = { Privilege.EmptyPrivilege };

        public static Privilege[] AuthorPrivileges = { Privilege.EmptyPrivilege, Privilege.ManageAuthorYourselfPrivilege,
            Privilege.AddDocumentPrivilege, Privilege.ManageDocumentPrivilege };

        public static Privilege[] AdminPrivileges = { Privilege.EmptyPrivilege, Privilege.SeeAllUsersPrivilege,
            Privilege.ManageUserPrivilege, Privilege.DeleteUserPrivilege, Privilege.SeeAllAuthorsPrivilege, Privilege.AddAuthorsPrivilege, 
            Privilege.ManageAuthorsPrivilege, Privilege.DeleteAuthorPrivilege, Privilege.EditAuthorAccountPrivilege, Privilege.SeeAllDocumentsPrivilege,
            Privilege.AddDocumentPrivilege, Privilege.ManageDocumentPrivilege, Privilege.DeleteDocumentPrivilege, Privilege.AddCategoryPrivilege,
            Privilege.CanPublishArticle, Privilege.SeeArticleLogPrivilege, Privilege.SeeSiteFiles, Privilege.AddSiteFiles, Privilege.DeleteSiteFiles, 
            Privilege.AddPageTiles, Privilege.ModifyPageTiles, Privilege.DeletePageTiles, Privilege.SeePageTiles
        };

        public static Privilege[] SuperAdminPrivileges = { Privilege.EmptyPrivilege, Privilege.SeeAllUsersPrivilege,
            Privilege.ManageUserPrivilege, Privilege.DeleteUserPrivilege, Privilege.ManageAccountPermissionsPrivilege,
            Privilege.ManageAdminPermissionsPrivilege, Privilege.SeeAllAuthorsPrivilege, Privilege.AddAuthorsPrivilege, Privilege.ManageAuthorsPrivilege, 
            Privilege.DeleteAuthorPrivilege, Privilege.EditAuthorAccountPrivilege, Privilege.SeeAllDocumentsPrivilege,
            Privilege.AddDocumentPrivilege, Privilege.ManageDocumentPrivilege, Privilege.DeleteDocumentPrivilege, Privilege.AddCategoryPrivilege,
            Privilege.CanPublishArticle, Privilege.SeeArticleLogPrivilege, Privilege.SeeSiteFiles, Privilege.AddSiteFiles, Privilege.DeleteSiteFiles, 
            Privilege.AddPageTiles, Privilege.ModifyPageTiles, Privilege.DeletePageTiles, Privilege.SeePageTiles
        };
        

        public static IDictionary<string, Privilege[]> RoleCodePrivileges = new Dictionary<string, Privilege[]>();

        static RolesPrivileges()
        {
            RoleCodePrivileges.Add(Role.NOT_ACTIVATED_USER_ROLE_CODE, NonActiveUserPrivileges);
            RoleCodePrivileges.Add(Role.USER_ROLE_CODE, UserPrivileges);
            RoleCodePrivileges.Add(Role.AUTHOR_ROLE_CODE, AuthorPrivileges);
            RoleCodePrivileges.Add(Role.ADMIN_ROLE_CODE, AdminPrivileges);
            RoleCodePrivileges.Add(Role.SUPER_ADMIN_ROLE_CODE, SuperAdminPrivileges);
        }

        public static IList<Privilege> GetPrivileges(IList<string> rolesCodes)
        {
            IList<Privilege> resultPrivileges = new List<Privilege>();
            foreach (string roleCode in rolesCodes)
            {
                if (RoleCodePrivileges.ContainsKey(roleCode))
                {
                    foreach (Privilege privilege in RoleCodePrivileges[roleCode])
                    {
                        if (!resultPrivileges.Contains(privilege))
                            resultPrivileges.Add(privilege);
                    }
                }
            }

            return resultPrivileges;
        }
    }
}