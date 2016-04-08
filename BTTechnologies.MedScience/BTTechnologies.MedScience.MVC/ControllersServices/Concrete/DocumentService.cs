using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.App_LocalResources;
using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Helpers;
using BTTechnologies.MedScience.MVC.Infrastructure;
using BTTechnologies.MedScience.MVC.Infrastructure.Messages;
using BTTechnologies.MedScience.MVC.Infrastructure.Security;
using BTTechnologies.MedScience.MVC.Models;
using BTTechnologies.MedScience.MVC.QuickSearchString;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Concrete
{
    public class DocumentService : IDocumentService
    {
        private readonly IList<BTTAjaxGridHelper.ChangeValueInformation> valuesToChange = ServicesHelper.GetGridChangeBoolValueList();
        private readonly IDocumentsRepository documentsRepository;
        private readonly IMembershipService membershipService;

        public DocumentService(IDocumentsRepository newDocumentsRepository, IMembershipService newMembershipService)
        {
            documentsRepository = newDocumentsRepository;
            membershipService = newMembershipService;
        }

        public BTTAjaxOutputGridModel GetDocumentsData(BTTAjaxInputGridModel inputGridModel)
        {
            return BTTAjaxGridHelper.GetGridData(documentsRepository.Context.ArticlesFullDataRecords, inputGridModel, valuesToChange);
        }

        public BTTAjaxOutputGridModel GetSiteFilesData(BTTAjaxInputGridModel inputGridModel, UrlHelper urlHelper)
        {
            IList<BTTAjaxGridHelper.ChangeValueInformation> changeValues = new List<BTTAjaxGridHelper.ChangeValueInformation>
            {
                new BTTAjaxGridHelper.ChangeValueInformation(typeof (string), string.Empty, string.Empty)
                {
                    CompareDelegate = o => o != null && o.ToString().Contains("~/"),
                    GetValueDelegate = v => v == null ? null : FilesHelper.GetContentFullPath(urlHelper, v.ToString())
                }
            };

            return BTTAjaxGridHelper.GetGridData(documentsRepository.Context.SiteFiles, inputGridModel, changeValues);
        }

        public QuickSearchStringOutputModel QuickSearchForCategories(QuickSearchInputModel inputModel)
        {
            QuickSearchStringOutputModel outputModel = new QuickSearchStringOutputModel();
            IList<ArticleCategory> categories = documentsRepository.GetCategoryByNamePart(inputModel.CurrentValue);

            foreach (QuickSearchItemInfo categoryInfo in from category in categories select new QuickSearchItemInfo(category.DisplayName, category.CategoryDescription, category.Id))
            {
                outputModel.SearchResults.Add(categoryInfo);
            }

            return outputModel;
        }

        public string GetCategoryFullNameById(int id)
        {
            ArticleCategory category = documentsRepository.GetCategoryById(id);

            if (category == null)
                return string.Empty;

            return category.DisplayName;
        }

        public ManageArticleModel GetNewArticleModel(string authorSearchMethodUrl, string categorySearchMethodUrl)
        {
            QuickSearchStringModel authorSearchModel = new QuickSearchStringModel { SearchMethodUrl = authorSearchMethodUrl, QuickSearchInitString = MedSiteStrings.InputAuthorNameOrSurname, NothingFoundIsClickable = true, NothingFoundString = MedSiteStrings.AddNewAuthor };
            QuickSearchStringModel categorySearchModel = new QuickSearchStringModel { SearchMethodUrl = categorySearchMethodUrl, QuickSearchInitString = MedSiteStrings.InputCategory, NothingFoundIsClickable = true, NothingFoundString = MedSiteStrings.AddNewCategory };
            return new ManageArticleModel { AuthorQuickSearchData = authorSearchModel, CategoryQuickSearchData = categorySearchModel };
        }

        public ManageArticleModel GetArticleModelById(int id, string authorSearchMethodUrl, string categorySearchMethodUrl)
        {
            ManageArticleModel model = GetNewArticleModel(authorSearchMethodUrl, categorySearchMethodUrl);
            Article article = documentsRepository.GetArticleById(id);
            if (article == null)
                return model;

            if (!ModelsMapper.CreateNewModelUsingMapping(model, article))
                return model;

            foreach (DocAttachment attachment in article.Attachments)
            {
                model.ArticleAttachments.Add(new ArticleAttachmentModel(attachment.Id, attachment.DisplayName, attachment.AttachmentURL));
            }

            foreach (Author author in article.Authors)
            {
                model.ArticleAuthors.Add(new ArticleAuthorModel(author.Id, ServicesHelper.GetAuthorFullName(author)));
            }

            foreach (ArticleCategory category in article.Categories)
            {
                model.ArticleCategories.Add(new ArticleCategoryModel(category.Id, category.DisplayName));
            }

            return model;
        }

        public MedScienceErrors QuicklyAddCategory(QuickAddCategoryModel model, out int newId)
        {
            newId = 0;
            ArticleCategory category = documentsRepository.GetCategoryByDisplayName(model.DisplayName);

            if (category != null)
                return MedScienceErrors.CategoryAlreadyExistsError;

            ArticleCategory newCategory = new ArticleCategory();
            if (!ModelsMapper.CreateNewModelUsingMapping(newCategory, model))
                return MedScienceErrors.UnknownError;

            documentsRepository.AddOrUpdateCategory(newCategory);
            newId = newCategory.Id;

            return MedScienceErrors.NoError;
        }

        public MedScienceErrors RemoveDocument(int id, string baseDocumentsUploadDirectory)
        {
            Article article = documentsRepository.GetArticleById(id);
            if (article == null)
                return MedScienceErrors.UnknownError;

            try
            {
                IList<string> filesToDelete = article.Attachments.Select(docAttachment => docAttachment.AttachmentURL).ToList();

                documentsRepository.DeleteArticle(article);

                foreach (string url in filesToDelete)
                    FilesHelper.DeleteFile(url, baseDocumentsUploadDirectory);
            }
            catch (Exception e)
            {
                ExceptionsLogger.LogException(e);
                return MedScienceErrors.UnknownError;
            }

            return MedScienceErrors.NoError;
        }

        public MedScienceErrors RemoveFile(int id, string baseDocumentsUploadDirectory)
        {
            SiteFile file = documentsRepository.GetSiteFileById(id);
            if (file == null)
                return MedScienceErrors.UnknownError;
            
            try
            {
                documentsRepository.DeleteSiteFile(file);
                FilesHelper.DeleteFile(file.FileUrl, baseDocumentsUploadDirectory);
            }
            catch (Exception e)
            {
                ExceptionsLogger.LogException(e);
                return MedScienceErrors.UnknownError;
            }

            return MedScienceErrors.NoError;
        }

        public MedScienceErrors AddEditDocument(ManageArticleModel model, string userLogin, BTTAjaxFileUploader fileUploader, string baseDocumentsPath, out int id)
        {
            id = model.Id;

            try
            {
                Article article;
                MedScienceErrors error = GetArticleEntity(userLogin, model, out article);
                if (error != MedScienceErrors.NoError)
                    return error;

                IList<DocAttachment> attachmentsToDelete = DeleteAttachmentsFromArticle(article, model);
                IList<string> filesToDelete = attachmentsToDelete.Select(docAttachment => docAttachment.AttachmentURL).ToList();

                IList<ArticleCategory> categoriesToDelete = DeleteCategoriesFromArticle(article, model);
                DeleteAuthorsFromArticle(article, model);

                IEnumerable<AttachmentInfo> newAttachments = AddItemsToArticle(article, model, fileUploader, baseDocumentsPath);

                try
                {
                    documentsRepository.AddUpdateArticle(article, attachmentsToDelete, categoriesToDelete, userLogin);
                }
                catch (Exception)
                {
                    foreach (AttachmentInfo info in newAttachments.Where(info => File.Exists(info.FullPath)))
                    {
                        File.Delete(info.FullPath);
                    }

                    throw;
                }

                id = article.Id;
                
                foreach (string url in filesToDelete)
                {
                    FilesHelper.DeleteFile(url, baseDocumentsPath);
                }
                
                return error;
            }
            catch (Exception e)
            {
                ExceptionsLogger.LogException(e);
                return MedScienceErrors.UnknownError;
            }
        }

        public MedScienceErrors SaveUploadedFiles(UploadedFiles files, string uploadPath, BTTAjaxFileUploader fileUploader)
        {
            if (files == null || files.UploadedFilesGuids == null || !files.UploadedFilesGuids.Any() || string.IsNullOrEmpty(uploadPath))
                return MedScienceErrors.NoError;

            IList<SiteFile> newFiles = new List<SiteFile>();

            try
            {
                foreach (string uploadedFilesGuid in files.UploadedFilesGuids)
                {
                    Guid fileGuid;
                    if (!Guid.TryParse(uploadedFilesGuid, out fileGuid))
                        continue;

                    string filePath = fileUploader.MoveFileToDirectory(uploadPath, fileGuid);
                    newFiles.Add(new SiteFile {DisplayName = Path.GetFileName(filePath), FileUrl = FilesHelper.GetFileUrlByBasePath(uploadPath, filePath)});
                }

                documentsRepository.AddUploadedFiles(newFiles);
            }
            catch (Exception e)
            {
                ExceptionsLogger.LogException(e);
                return MedScienceErrors.UnknownError;
            }

            return MedScienceErrors.NoError;
        }

        private MedScienceErrors GetArticleEntity(string userLogin, ManageArticleModel model, out Article article)
        {
            article = null;

            if (string.IsNullOrEmpty(userLogin))
                return MedScienceErrors.UnknownError;

            if (string.IsNullOrEmpty(model.DisplayName))
                return MedScienceErrors.ArticleNameCannotBeEmpty;

            Article articleByName = documentsRepository.GetArtilceByName(model.DisplayName);
            if (articleByName != null && articleByName.Id != model.Id)
                return MedScienceErrors.ArticleWithSuchNameAlreadyExistsError;

            article = 0 < model.Id ? documentsRepository.GetArticleById(model.Id) : new Article();
            bool oldPublishedValue = article.Published;

            if (!ModelsMapper.CreateNewModelUsingMapping(article, model))
                return MedScienceErrors.UnknownError;

            if (!membershipService.UserHasPrivilege(userLogin, Privilege.CanPublishArticle))
            {
                article.Published = article.Id > 0 && oldPublishedValue;
            }

            return MedScienceErrors.NoError;
        }

        private IEnumerable<AttachmentInfo> AddItemsToArticle(Article article, ManageArticleModel model, BTTAjaxFileUploader fileUploader, string baseDocumentsPath)
        {
            if (model.AuthorsIds != null)
            {
                foreach (int authorId in model.AuthorsIds.Where(authorId => article.Authors.All(a => a.Id != authorId)))
                {
                    Author author = documentsRepository.GetAuthorById(authorId);
                    if (author != null)
                        article.Authors.Add(author);
                }
            }

            if (model.CategoriesIds != null)
            {
                foreach (int id in model.CategoriesIds.Where(i => article.Categories.All(c => c.Id != i)))
                {
                    ArticleCategory category = documentsRepository.GetCategoryById(id);
                    if (category != null)
                        article.Categories.Add(category);
                }
            }

            IList<AttachmentInfo> attachments = new List<AttachmentInfo>();

            if (model.UploadedFilesGuids != null)
            {
                string directoryName = ServicesHelper.GetValidDirectoryName(article.DisplayName);
                string uploadDirectory = FilesHelper.GetUploadFileDirectoryName(baseDocumentsPath, directoryName);

                foreach (string uploadedFilesGuid in model.UploadedFilesGuids)
                {
                    Guid fileGuid;
                    if (!Guid.TryParse(uploadedFilesGuid, out fileGuid))
                        continue;

                    string filePath = fileUploader.MoveFileToDirectory(uploadDirectory, fileGuid);

                    DocAttachment attachment = new DocAttachment
                                                   {
                                                       DisplayName = Path.GetFileNameWithoutExtension(filePath),
                                                       AttachmentType = Path.GetExtension(filePath),
                                                       AttachmentURL = FilesHelper.GetFileUrlByBasePath(baseDocumentsPath, filePath),
                                                       DownloadOptions = string.Empty
                                                   };
                
                    article.Attachments.Add(attachment);
                    attachments.Add(new AttachmentInfo(attachment, filePath));
                }
            }

            return attachments;
        }

        private IList<DocAttachment> DeleteAttachmentsFromArticle(Article article, ManageArticleModel model)
        {
            IList<DocAttachment> attachmentsToDelete = model.DocumentsIds == null ? article.Attachments.ToList(): article.Attachments.Where(a => !model.DocumentsIds.Contains(a.Id)).ToList();

            foreach (DocAttachment docAttachment in attachmentsToDelete)
            {
                article.Attachments.Remove(docAttachment);
            }

            return attachmentsToDelete;
        }

        private IList<ArticleCategory> DeleteCategoriesFromArticle(Article article, ManageArticleModel model)
        {
            IList<ArticleCategory> categoriesToDelete = model.CategoriesIds == null ? article.Categories.ToList(): article.Categories.Where(cat => !model.CategoriesIds.Contains(cat.Id)).ToList();

            foreach (ArticleCategory articleCategory in categoriesToDelete)
            {
                article.Categories.Remove(articleCategory);
            }

            return categoriesToDelete;
        }    
        
        private void DeleteAuthorsFromArticle(Article article, ManageArticleModel model)
        {
            IList<Author> authorsToDelete = model.AuthorsIds == null ? article.Authors.ToList(): article.Authors.Where(author => !model.AuthorsIds.Contains(author.Id)).ToList();

            foreach (Author author in authorsToDelete)
            {
                article.Authors.Remove(author);
            }
        }
        
        private class AttachmentInfo
        {
            public DocAttachment Attachment { get; private set; }
            public string FullPath { get; private set; }

            public AttachmentInfo(DocAttachment attachment, string fullPath)
            {
                Attachment = attachment;
                FullPath = fullPath;
            }
        }
    }
}