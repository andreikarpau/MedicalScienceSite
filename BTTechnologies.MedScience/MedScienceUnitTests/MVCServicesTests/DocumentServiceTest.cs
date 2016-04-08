using System;
using System.Collections.Generic;
using System.IO;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.ControllersServices.Concrete;
using BTTechnologies.MedScience.MVC.Helpers;
using BTTechnologies.MedScience.MVC.Infrastructure.Messages;
using BTTechnologies.MedScience.MVC.Infrastructure.Security;
using BTTechnologies.MedScience.MVC.Models;
using MedScienceUnitTests.Helpers;
using MedScienceUnitTests.MVCHelpersTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace MedScienceUnitTests.MVCServicesTests
{
    [TestClass]
    public class DocumentServiceTest
    {
        private readonly BTTAjaxFileUploader fileUploader = new BTTAjaxFileUploader();
        private static bool addUpdateArticleThrowException;

        private static IDocumentService documentService;
        private const string login = "TestLogin";
        private const string adminLogin = "AdminLogin";

        const string fileName1 = "tempTestFile1.doc";
        const string fileName2 = "tempTestFile2.doc";
        const string fileName3 = "tempTestFile3.doc";

        const string tempArticleName = "One Article";
        const string tempArticleContent = "This is all article content";
        const string tempArticleDescription = "One article description";

        static readonly IEnumerable<int> TempAuthorIds = new[] { 1, 2 };
        static readonly IEnumerable<int> TempCategoriesIds = new[] { 5, 6 };

        private static readonly IList<Article> ArticleInDatabase = new List<Article>
                                                                       {
                                                                           new Article { Id = 1, DisplayName = "Article1", DocumentDescription = "Document Article1 description" },
                                                                           new Article { Id = 2, DisplayName = "Article2", DocumentDescription = "Document Article2 description", Published = true },
                                                                           new Article { 
                                                                               Id = 3, 
                                                                               DisplayName = "Article3", 
                                                                               DocumentDescription = "Document Article3 description",
                                                                               Authors = new List<Author> { new Author{Id = 1, Name = GetAuthorNameById(1)}, new Author{Id = 2, Name = GetAuthorNameById(2)}},
                                                                               Categories = new List<ArticleCategory> {new ArticleCategory{Id = 5, DisplayName = GetCategoryNameById(5)}, new ArticleCategory{Id = 6, DisplayName = GetCategoryNameById(6)}}
                                                                           },
                                                                           new Article {
                                                                               Id = 4, 
                                                                               DisplayName = "Инструкция по применению 230-3-1212 Метод ведения пациентов с сочетанной инфекцией: гепатиты В, С и ВИЧ-инфекция", 
                                                                               DocumentDescription = "Document Article4 description"
                                                                           }
                                                                       };

        private static readonly IList<Account> AccountsList = new List<Account>
                                                                  {
                                                                    new Account {Id = 1, UserLogin = adminLogin, Roles = new List<Role> { new Role {Code = Role.ADMIN_ROLE_CODE} }},
                                                                    new Account {Id = 2, UserLogin = "author@a.com", Roles = new List<Role> { new Role {Code = Role.AUTHOR_ROLE_CODE} }}
                                                                  };

        private static readonly IList<int> AuthorExistedIds = new List<int> {1, 3, 4, 5}; 
        private static readonly IList<int> CategoriesExistedIds = new List<int> {1, 3, 4, 5}; 

        private static readonly IList<UpdatedArticleInfo> UpdatedArticles = new List<UpdatedArticleInfo>();
        private static readonly IList<SiteFile> AddedFiles = new List<SiteFile>();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Mock<IDocumentsRepository> documentsRepositoryMock = new Mock<IDocumentsRepository>();
            documentsRepositoryMock.Setup(r => r.GetArtilceByName(It.IsAny<string>())).Returns<string>(n => ArticleInDatabase.FirstOrDefault(a => a.DisplayName == n));
            documentsRepositoryMock.Setup(r => r.GetArticleById(It.IsAny<int>())).Returns<int>(n => ArticleInDatabase.FirstOrDefault(a => a.Id == n));
            documentsRepositoryMock.Setup(r => r.AddUpdateArticle(It.IsAny<Article>(), It.IsAny<IList<DocAttachment>>(), It.IsAny<IList<ArticleCategory>>(), It.IsAny<string>())).Callback((Article article, IList<DocAttachment> oldAttachments, IList<ArticleCategory> oldCategories, string userLogin) => AddUpdateArticle(article, oldAttachments, oldCategories, userLogin));
            documentsRepositoryMock.Setup(r => r.GetAuthorById(It.IsAny<int>())).Returns<int>(i => AuthorExistedIds.Contains(i) ? new Author{Id = i, Name = GetAuthorNameById(i) }: null);
            documentsRepositoryMock.Setup(r => r.GetCategoryById(It.IsAny<int>())).Returns<int>(i => CategoriesExistedIds.Contains(i) ? new ArticleCategory { Id = i, DisplayName = GetCategoryNameById(i) } : null);
            documentsRepositoryMock.Setup(r => r.GetSiteFileById(It.IsAny<int>())).Returns<int>(i => AddedFiles.FirstOrDefault(f => f.Id == i));
            documentsRepositoryMock.Setup(r => r.DeleteSiteFile(It.IsAny<SiteFile>())).Callback<SiteFile>(file =>
            {
                if (AddedFiles.Contains(file))
                    AddedFiles.Remove(file);
            });
            documentsRepositoryMock.Setup(r => r.AddUploadedFiles(It.IsAny<IEnumerable<SiteFile>>())).Callback<IEnumerable<SiteFile>>(s =>
            {
                foreach (SiteFile file in s)
                {
                    file.Id = AddedFiles.Count + 1;
                    AddedFiles.Add(file);
                }
            });
            
            Mock<IMembershipService> membershipServiceMock = new Mock<IMembershipService>();
            membershipServiceMock.Setup(r => r.UserHasPrivilege(It.IsAny<string>(), It.IsAny<Privilege[]>())).Returns((string s, Privilege[] p) => UserHasPrivilege(s, p));

            documentService = new DocumentService(documentsRepositoryMock.Object, membershipServiceMock.Object);
        }

        [TestMethod]
        public void AddEditDocumentVerificationTest()
        {
            int resultId;

            UpdatedArticles.Clear();
            ManageArticleModel model = new ManageArticleModel {Id = 12};
            MedScienceErrors error = documentService.AddEditDocument(model, string.Empty, fileUploader, string.Empty, out resultId);

            Assert.AreEqual(error, MedScienceErrors.UnknownError);
            Assert.IsFalse(UpdatedArticles.Any());

            error = documentService.AddEditDocument(model, login, fileUploader, string.Empty, out resultId);
            Assert.AreEqual(error, MedScienceErrors.ArticleNameCannotBeEmpty);
            Assert.IsFalse(UpdatedArticles.Any());

            model.DisplayName = "Article1";
            error = documentService.AddEditDocument(model, login, fileUploader, string.Empty, out resultId);
            Assert.AreEqual(error, MedScienceErrors.ArticleWithSuchNameAlreadyExistsError);
            Assert.IsFalse(UpdatedArticles.Any());
        }

        [TestMethod]
        public void AddEditDocumentPublishFieldTest()
        {
            int resultId;
            ManageArticleModel model = new ManageArticleModel { Id = 1, DisplayName = "Article1", Published = true };

            UpdatedArticles.Clear();
            documentService.AddEditDocument(model, login, fileUploader, string.Empty, out resultId);
            
            Assert.AreEqual(UpdatedArticles.Count, 1);
            Assert.AreEqual(UpdatedArticles.First().Article.Published, false);

            model = new ManageArticleModel { Id = 1, DisplayName = "Article1", Published = false };
            UpdatedArticles.Clear();
            documentService.AddEditDocument(model, login, fileUploader, string.Empty, out resultId);

            Assert.AreEqual(UpdatedArticles.Count, 1);
            Assert.AreEqual(UpdatedArticles.First().Article.Published, false);

            model = new ManageArticleModel { Id = 2, DisplayName = "Article2", Published = false };
            UpdatedArticles.Clear();
            documentService.AddEditDocument(model, login, fileUploader, string.Empty, out resultId);

            Assert.AreEqual(UpdatedArticles.Count, 1);
            Assert.AreEqual(UpdatedArticles.First().Article.Published, true);

            model = new ManageArticleModel { Id = 0, DisplayName = "Article15", Published = true };
            UpdatedArticles.Clear();
            documentService.AddEditDocument(model, login, fileUploader, string.Empty, out resultId);

            Assert.AreEqual(UpdatedArticles.Count, 1);
            Assert.AreEqual(UpdatedArticles.First().Article.Published, false);

            model = new ManageArticleModel { Id = 2, DisplayName = "Article2", Published = false };
            UpdatedArticles.Clear();
            documentService.AddEditDocument(model, adminLogin, fileUploader, string.Empty, out resultId);

            Assert.AreEqual(UpdatedArticles.Count, 1);
            Assert.AreEqual(UpdatedArticles.First().Article.Published, false);
        }

        [TestMethod]
        public void AddEditDocumentInsertNewTest()
        {
            int resultId;
            // Min data required insert
            UpdatedArticles.Clear();
            ManageArticleModel model = new ManageArticleModel { DisplayName = tempArticleName, DocumentDescription = tempArticleDescription };
            documentService.AddEditDocument(model, login, fileUploader, string.Empty, out resultId);
            Assert.AreEqual(UpdatedArticles.Count, 1);
            CheckUpdatedArticleInfo(UpdatedArticles.First(), new ArticleDataToCompare(resultId, false, model));

            // No uploaded files
            UpdatedArticles.Clear();
            model = new ManageArticleModel { Id = 0, DisplayName = tempArticleName, AuthorsIds = TempAuthorIds, CategoriesIds = TempCategoriesIds, Content = tempArticleContent, DocumentDescription = tempArticleDescription };
            documentService.AddEditDocument(model, login, fileUploader, string.Empty, out resultId);
            Assert.AreEqual(UpdatedArticles.Count, 1);
            CheckUpdatedArticleInfo(UpdatedArticles.First(), new ArticleDataToCompare(resultId, false));

            // Do upload files
            using (TempDirectoryProvider uploaderDirectoryProvider = new TempDirectoryProvider())
            {
                BTTAjaxFileUploader currentUploader;
                UploadFilesOutputModel outputModel = SaveFilesByUploader(uploaderDirectoryProvider.TempDirectoryPath, out currentUploader, fileName1, fileName2, fileName3);

                using (TempDirectoryProvider directoryProvider = new TempDirectoryProvider())
                {
                    UpdatedArticles.Clear();
                    model.UploadedFilesGuids = outputModel.UploadedFileInfos.Select(i => i.FileGuid.ToString());
                    documentService.AddEditDocument(model, adminLogin, currentUploader, directoryProvider.TempDirectoryPath, out resultId);

                    Assert.AreEqual(UpdatedArticles.Count, 1);
                    CheckFilesExist(Path.Combine(directoryProvider.TempDirectoryPath, UpdatedArticles.First().Article.DisplayName), fileName1, fileName2, fileName3);
                }
            }

            // Do upload files with exception
            using (TempDirectoryProvider uploaderDirectoryProvider = new TempDirectoryProvider())
            {
                addUpdateArticleThrowException = true;

                BTTAjaxFileUploader currentUploader;
                UploadFilesOutputModel outputModel = SaveFilesByUploader(uploaderDirectoryProvider.TempDirectoryPath, out currentUploader, fileName1, fileName2, fileName3);

                using (TempDirectoryProvider directoryProvider = new TempDirectoryProvider())
                {
                    UpdatedArticles.Clear();
                    model.UploadedFilesGuids = outputModel.UploadedFileInfos.Select(i => i.FileGuid.ToString());
                    documentService.AddEditDocument(model, adminLogin, currentUploader, directoryProvider.TempDirectoryPath, out resultId);

                    Assert.AreEqual(UpdatedArticles.Count, 0);
                    Assert.IsFalse(File.Exists(Path.Combine(directoryProvider.TempDirectoryPath, model.DisplayName, fileName1)));
                    Assert.IsFalse(File.Exists(Path.Combine(directoryProvider.TempDirectoryPath, model.DisplayName, fileName2)));
                    Assert.IsFalse(File.Exists(Path.Combine(directoryProvider.TempDirectoryPath, model.DisplayName, fileName3)));
                }

                addUpdateArticleThrowException = false;
            }
        }

        [TestMethod]
        public void AddEditDocumentUpdateTest()
        {
            int resultId;

            const string newName = "NewArticleName";
            const string newDescription = "NewArticleDescription";

            //Delete old categories and authors
            UpdatedArticles.Clear();
            ManageArticleModel model = new ManageArticleModel { Id = 3, 
                DisplayName = newName, AuthorsIds = TempAuthorIds, CategoriesIds = TempCategoriesIds, Content = tempArticleContent, 
                DocumentDescription = newDescription, Published = true };

            model.AuthorsIds = new [] {1};
            model.CategoriesIds = new [] {5};

            documentService.AddEditDocument(model, adminLogin, fileUploader, string.Empty, out resultId);
            Assert.AreEqual(UpdatedArticles.Count, 1);
            CheckUpdatedArticleInfo(UpdatedArticles.First(), new ArticleDataToCompare(resultId, true, model));

            Assert.AreEqual(UpdatedArticles.First().OldCategories.Count, 1);
            Assert.AreEqual(UpdatedArticles.First().OldCategories.First().Id, 6);


            //Delete old attachments
            using (TempDirectoryProvider uploaderDirectoryProvider = new TempDirectoryProvider())
            {
                BTTAjaxFileUploader currentUploader;
                const string existedFileName1 = "ExistedFile1";
                const string existedFileName2 = "ExistedFile2.doc";
                const string existedFileName3ReadOnly = "ExistedReadOnlyFile3.doc";
                UploadFilesOutputModel outputModel = SaveFilesByUploader(uploaderDirectoryProvider.TempDirectoryPath, out currentUploader, existedFileName1, existedFileName2, existedFileName3ReadOnly);

                Article article = ArticleInDatabase.First(a => a.Id == 4);
                model = new ManageArticleModel
                {
                    Id = 4,
                    DisplayName = article.DisplayName,
                    DocumentDescription = article.DocumentDescription,
                    UploadedFilesGuids = outputModel.UploadedFileInfos.Select(i => i.FileGuid.ToString())
                };

                using (TempDirectoryProvider directoryProvider = new TempDirectoryProvider())
                {
                    UpdatedArticles.Clear();
                    documentService.AddEditDocument(model, adminLogin, currentUploader, directoryProvider.TempDirectoryPath, out resultId);
    
                    Assert.AreEqual(UpdatedArticles.Count, 1);
                    CheckFilesExist(Path.Combine(directoryProvider.TempDirectoryPath, ServicesHelper.GetValidDirectoryName(model.DisplayName)), existedFileName1, existedFileName2, existedFileName3ReadOnly);

                    File.SetAttributes(Path.Combine(directoryProvider.TempDirectoryPath, ServicesHelper.GetValidDirectoryName(model.DisplayName), existedFileName3ReadOnly), FileAttributes.ReadOnly);

                    outputModel = SaveFilesByUploader(uploaderDirectoryProvider.TempDirectoryPath, out currentUploader, fileName3);

                    IList<int> ids = new List<int>();
                    ids.Add(UpdatedArticles.First().Article.Attachments.First().Id);
                    model.DocumentsIds = ids;
                    model.UploadedFilesGuids = outputModel.UploadedFileInfos.Select(i => i.FileGuid.ToString());

                    UpdatedArticles.Clear(); 
                    documentService.AddEditDocument(model, adminLogin, currentUploader, directoryProvider.TempDirectoryPath, out resultId);

                    Assert.AreEqual(UpdatedArticles.Count, 1);
                    Assert.AreEqual(UpdatedArticles.First().Article.Attachments.Count, 2);
                    CheckFilesExist(Path.Combine(directoryProvider.TempDirectoryPath, ServicesHelper.GetValidDirectoryName(model.DisplayName)), existedFileName1, fileName3);
                    
                    Assert.AreEqual(UpdatedArticles.First().OldAttachments.Count, 2);
                    Assert.AreEqual(UpdatedArticles.First().OldAttachments[0].DisplayName, Path.GetFileNameWithoutExtension(existedFileName2));
                    Assert.AreEqual(UpdatedArticles.First().OldAttachments[1].DisplayName, Path.GetFileNameWithoutExtension(existedFileName3ReadOnly));

                    Assert.IsFalse(File.Exists(Path.Combine(directoryProvider.TempDirectoryPath, ServicesHelper.GetValidDirectoryName(model.DisplayName), existedFileName2)));
                    Assert.IsFalse(File.Exists(Path.Combine(directoryProvider.TempDirectoryPath, ServicesHelper.GetValidDirectoryName(model.DisplayName), existedFileName3ReadOnly)));
                }
            }
        }
        
        [TestMethod]
        public void AddDeleteSiteFileTest()
        {
            AddedFiles.Clear();

            //Delete old attachments
            using (TempDirectoryProvider uploaderDirectoryProvider = new TempDirectoryProvider())
            {
                BTTAjaxFileUploader currentUploader;
                const string existedFileName1 = "ExistedFile1";
                const string existedFileName2 = "ExistedFile2.doc";
                IList<string> fileNames = new List<string> { existedFileName1, existedFileName2 };

                UploadFilesOutputModel outputModel = SaveFilesByUploader(uploaderDirectoryProvider.TempDirectoryPath, out currentUploader, existedFileName1, existedFileName2);
                
                using (TempDirectoryProvider directoryProvider = new TempDirectoryProvider())
                {
                    UploadedFiles uploadedFiles = new UploadedFiles {UploadedFilesGuids = outputModel.UploadedFileInfos.Select(i => i.FileGuid.ToString())};
                    documentService.SaveUploadedFiles(uploadedFiles, directoryProvider.TempDirectoryPath, currentUploader);

                    Assert.AreEqual(AddedFiles.Count, 2);
                    Assert.AreNotEqual(AddedFiles[0].DisplayName, AddedFiles[1].DisplayName);

                    foreach (SiteFile file in AddedFiles)
                    {
                        Assert.IsTrue(fileNames.Contains(file.DisplayName));

                        string baseFilesDirectory = directoryProvider.TempDirectoryPath.Split('\\').Last();
                        string urlPath = "~/" + baseFilesDirectory + "/" + file.DisplayName;

                        Assert.IsTrue(string.Equals(file.FileUrl, urlPath, StringComparison.OrdinalIgnoreCase));
                        Assert.IsTrue(File.Exists(Path.Combine(directoryProvider.TempDirectoryPath, file.DisplayName)));
                    }


                    SiteFile fileToDelete = AddedFiles.Last();
                    documentService.RemoveFile(fileToDelete.Id, directoryProvider.TempDirectoryPath);

                    Assert.IsFalse(File.Exists(Path.Combine(directoryProvider.TempDirectoryPath, fileToDelete.DisplayName)));

                    SiteFile remainingFile = AddedFiles.First();
                    Assert.IsTrue(File.Exists(Path.Combine(directoryProvider.TempDirectoryPath, remainingFile.DisplayName)));
                }
            }
        }

        private void CheckFilesExist(string filesDirectory, params string[] files)
        {
            string possibleDirectoryName = filesDirectory; 
            Assert.IsTrue(Directory.Exists(possibleDirectoryName));

            string baseFilesDirectory = possibleDirectoryName.Split('\\')[possibleDirectoryName.Split('\\').Length - 2];
            string articleDirectory = possibleDirectoryName.Split('\\')[possibleDirectoryName.Split('\\').Length - 1];

            foreach (string file in files)
            {
                Assert.IsTrue(File.Exists(Path.Combine(possibleDirectoryName, file)));
                
                string urlPath = "~/" + baseFilesDirectory + "/" + articleDirectory + '/' + file;

                IEnumerable<DocAttachment> attachments = UpdatedArticles.First().Article.Attachments.Where(a => urlPath.Equals(a.AttachmentURL, StringComparison.InvariantCultureIgnoreCase));
                Assert.AreEqual(attachments.Count(), 1);
                Assert.AreEqual(attachments.First().AttachmentType, Path.GetExtension(file));
                Assert.AreEqual(attachments.First().DisplayName, Path.GetFileNameWithoutExtension(file));
            }
        }

        private UploadFilesOutputModel SaveFilesByUploader(string directory, out BTTAjaxFileUploader uploader, params string[] files)
        {
            uploader = new Tested(directory);

            UploadFilesInputModel inputModel = new UploadFilesInputModel();
            foreach (string file in files)
            {
                inputModel.PostedFiles.Add(file, null);
            }

            return uploader.SaveFilesToTempDirectory(inputModel);
        }

        private static void CheckUpdatedArticleInfo(UpdatedArticleInfo info, ArticleDataToCompare dataToCompare)
        {
            Assert.AreEqual(info.Article.Content, dataToCompare.Content);
            Assert.AreEqual(info.Article.DisplayName, dataToCompare.DisplayName);
            Assert.AreEqual(info.Article.DocumentDescription, dataToCompare.DocumentDescription);
            Assert.AreEqual(info.Article.Id, dataToCompare.Id);
            Assert.AreNotEqual(info.Article.LastChangedDate, default(DateTime));
            Assert.AreEqual(info.Article.Published, dataToCompare.Published);

            if (dataToCompare.CategoriesIds != null)
            {
                IEnumerable<int> categoriesExisted =
                    dataToCompare.CategoriesIds.Where(i => CategoriesExistedIds.Contains(i));
                Assert.AreEqual(info.Article.Categories.Count, categoriesExisted.Count());

                foreach (ArticleCategory articleCategory in info.Article.Categories)
                {
                    Assert.AreEqual(categoriesExisted.Count(i => i == articleCategory.Id), 1);
                    Assert.AreEqual(articleCategory.DisplayName, GetCategoryNameById(articleCategory.Id));
                }
            }

            if (dataToCompare.AuthorIds != null)
            {
                IEnumerable<int> authorsExisted = dataToCompare.AuthorIds.Where(i => AuthorExistedIds.Contains(i));
                Assert.AreEqual(info.Article.Authors.Count, authorsExisted.Count());

                foreach (Author author in info.Article.Authors)
                {
                    Assert.AreEqual(authorsExisted.Count(i => i == author.Id), 1);
                    Assert.AreEqual(author.Name, GetAuthorNameById(author.Id));
                }
            }
        }

        private static string GetCategoryNameById(int id)
        {
            return string.Format("{0}{1}", "Category", id);
        }

        private static string GetAuthorNameById(int id)
        {
            return string.Format("{0}{1}", "Author", id);
        }

        private static void AddUpdateArticle(Article article, IList<DocAttachment> oldAttachments, IList<ArticleCategory> oldCategories, string userLogin)
        {
            if (addUpdateArticleThrowException)
                throw new Exception();

            if (article.Id == 0)
                article.Id = 123;

            article.LastChangedDate = DateTime.Now;

            int i = 1;
            foreach (DocAttachment attachment in article.Attachments)
            {

                if (attachment.Id == 0)
                    attachment.Id = i;

                i++;
            }

            UpdatedArticles.Add(new UpdatedArticleInfo(article, oldAttachments, oldCategories, userLogin));
        }
        
        private static bool UserHasPrivilege(string userLogin, IEnumerable<Privilege> privileges)
        {
            Account account = AccountsList.FirstOrDefault(a => a.UserLogin == userLogin);
            if (account == null)
                return false;

            if (privileges == null || !privileges.Any())
                return true;

            return account.Roles.Any(r => r.Code == Role.ADMIN_ROLE_CODE);
        }

        private class ArticleDataToCompare
        {
            public string Content { get; private set; }
            public string DisplayName { get; private set; }
            public string DocumentDescription { get; private set; }
            public int Id { get; private set; }
            public bool Published { get; private set; }
            public IEnumerable<int> AuthorIds { get; private set; }
            public IEnumerable<int> CategoriesIds { get; private set; }

            public ArticleDataToCompare(int newId, bool published)
            {
                Content = tempArticleContent;
                DisplayName = tempArticleName;
                DocumentDescription = tempArticleDescription;
                Id = newId;
                Published = published;
                AuthorIds = TempAuthorIds;
                CategoriesIds = TempCategoriesIds;
            }

            public ArticleDataToCompare(int newId, bool published, ManageArticleModel model)
            {
                Content = model.Content;
                DisplayName = model.DisplayName;
                DocumentDescription = model.DocumentDescription;
                Id = newId;
                Published = published;
                AuthorIds = model.AuthorsIds;
                CategoriesIds = model.CategoriesIds;
            }
        }

        private class UpdatedArticleInfo
        {
            public Article Article { get; private set; }
            public IList<DocAttachment> OldAttachments { get; private set; }
            public IList<ArticleCategory> OldCategories { get; private set; }
            public string UserLogin { get; set; }

            public UpdatedArticleInfo(Article article, IList<DocAttachment> oldAttachments, IList<ArticleCategory> oldCategories, string userLogin)
            {
                Article = article;
                OldAttachments = oldAttachments;
                OldCategories = oldCategories;
                UserLogin = userLogin;
            }
        }
    }
}
