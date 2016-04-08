using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.App_LocalResources;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.ControllersServices.Concrete;
using BTTechnologies.MedScience.MVC.Helpers;
using BTTechnologies.MedScience.MVC.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MedScienceUnitTests.MVCServicesTests
{
    [TestClass]
    public class ReadArticlesServiceTest
    {
        private const string someArticleNameStartPrefix = "articleNamePrefix";
        private static IReadArticlesService service;

        private static readonly IList<Article> Articles = new List<Article>
                                              {
                                                  new Article {DisplayName = someArticleNameStartPrefix + "article 1", Id = 1, Authors = new List<Author> {new Author{Surname = "aaa", Name = "vvv", Patronymic = "bbb"}}, CreatedDate = new DateTime(2001, 11, 5), LastChangedDate = new DateTime(2003, 12, 12)},
                                                  new Article {DisplayName = someArticleNameStartPrefix + "article 2", DocumentDescription = "Description a2 " + someArticleNameStartPrefix + " ddd", Id = 2, LastChangedDate = new DateTime(2013, 12, 11), CreatedDate = new DateTime(2001, 01, 5) },
                                                  new Article {DisplayName = "article 3", DocumentDescription = "Description " + someArticleNameStartPrefix + "aaa", Id = 3, LastChangedDate = new DateTime(2013, 11, 12), CreatedDate = new DateTime(2011, 12, 5)},
                                                  new Article {DisplayName = "article 4", Id = 4, CreatedDate = new DateTime(2011, 11, 5), LastChangedDate = new DateTime(2012, 12, 13)},
                                                  new Article {DisplayName = "article 5", Id = 5, CreatedDate = new DateTime(2012, 11, 5), LastChangedDate = new DateTime(2012, 12, 12)},
                                                  new Article {DisplayName = "article 6", Id = 6, CreatedDate = new DateTime(2013, 11, 5), LastChangedDate = new DateTime(2013, 12, 12)},
                                                  new Article {DisplayName = someArticleNameStartPrefix + "article 7", Id = 7, CreatedDate = new DateTime(2014, 11, 5), LastChangedDate = new DateTime(2014, 12, 12)},
                                                  new Article {DisplayName = "article 8", DocumentDescription = "descr 1111", LastChangedDate = new DateTime(2013, 12, 12), Id = 8, CreatedDate = new DateTime(2001, 11, 1) }
                                              };

        private static readonly IList<Author> Authors = new List<Author>
                                            {
                                                new Author{Name = "Name1", Surname = "Surname 1", Patronymic = "Patr1", Id = 1, Articles = Articles.Where(a => a.Id <= 2).ToList()},
                                                new Author{Name = "Name2", Surname = "SurName1 2", Patronymic = "Patr2", Id = 2,  Articles = Articles.Where(a => 3 <= a.Id && a.Id <= 5).ToList()},
                                                new Author{Name = "Name1", Surname = "Surname 3", Patronymic = "Patr3", Id = 3, Articles = new List<Article>()}
                                            };

        private static readonly IList<ArticleCategory> Categories = new List<ArticleCategory>
                                            {
                                                new ArticleCategory {DisplayName = "Cat1", CategoryDescription = "Descr1", Id = 1, Articles = Articles.Where(a => a.Id <= 2).ToList()},
                                                new ArticleCategory {DisplayName = "Cat2", CategoryDescription = "Descr2", Id = 2, Articles = Articles.Where(a => 3 <= a.Id && a.Id <= 5).ToList()}
                                            };

        private static readonly Article ArticleWithAttachmentsAndCategories = new Article
                                                                                  {
                                                                                      Id = 1,
                                                                                      DisplayName = "Article with info",
                                                                                      DocumentDescription = "This is description",
                                                                                      LastChangedDate = new DateTime(2013, 10, 11),
                                                                                      Content = "Article content aaaa aa a s s  d sad sa das d",
                                                                                      Published = true,
                                                                                      Attachments = new Collection<DocAttachment>
                                                                                                        {
                                                                                                            new DocAttachment {DisplayName = "DocAttachment1", Id = 1, AttachmentType = ".pdf"},
                                                                                                            new DocAttachment {DisplayName = "DocAttachment3", Id = 3 },
                                                                                                            new DocAttachment {DisplayName = "DocAttachment5", Id = 5, AttachmentType = ".doc"}
                                                                                                        },
                                                                                      Authors = new Collection<Author>
                                                                                                   {
                                                                                                       new Author { Name = "Alex", Patronymic = "Petrovich", Surname = "Romanov", Id = 2},
                                                                                                       new Author {Name = "Petr", Patronymic = "Vasilevich", Surname = "Sigaev", Id = 14}
                                                                                                   },
                                                                                      Categories = new Collection<ArticleCategory>
                                                                                                                    {
                                                                                                                        new ArticleCategory {DisplayName = "Inf des", Id = 12},
                                                                                                                        new ArticleCategory {DisplayName = "history", Id = 11}
                                                                                                                    }
                                                                                  };


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            foreach (Author author in Authors)
            {
                Articles[7].Authors.Add(author);
            }

            Mock<IArticlesRepository> articlesRepositoryMock = new Mock<IArticlesRepository>();
            articlesRepositoryMock.Setup(r => r.GetArticlesByNamePatternOrderByName(It.Is<string>(s => string.IsNullOrEmpty(s)))).Returns(new EnumerableQuery<Article>(Articles));
            articlesRepositoryMock.Setup(r => r.GetArticlesByNamePatternOrderByName(It.Is<string>(s => !string.IsNullOrEmpty(s)))).Returns((string s) => new EnumerableQuery<Article>(Articles.Where(a => a.DisplayName.StartsWith(s))));

            articlesRepositoryMock.Setup(r => r.GetArticlesByNamePatternOrderByModificationDate(It.Is<string>(s => !string.IsNullOrEmpty(s)))).Returns((string s) => new EnumerableQuery<Article>(Articles.Where(a => a.DisplayName.StartsWith(s)).OrderByDescending(a => a.LastChangedDate)));
            articlesRepositoryMock.Setup(r => r.GetArticlesByNamePatternOrderByCreatedDate(It.Is<string>(s => !string.IsNullOrEmpty(s)))).Returns((string s) => new EnumerableQuery<Article>(Articles.Where(a => a.DisplayName.StartsWith(s)).OrderByDescending(a => a.CreatedDate)));

            articlesRepositoryMock.Setup(r => r.GetArticlesByNamePatternOrderByModificationDate(It.Is<string>(s => string.IsNullOrEmpty(s)))).Returns((string s) => new EnumerableQuery<Article>(Articles.OrderByDescending(a => a.LastChangedDate)));
            articlesRepositoryMock.Setup(r => r.GetArticlesByNamePatternOrderByCreatedDate(It.Is<string>(s => string.IsNullOrEmpty(s)))).Returns((string s) => new EnumerableQuery<Article>(Articles.OrderByDescending(a => a.CreatedDate)));
            
            articlesRepositoryMock.Setup(repository => repository.GetPublishedArticleById(1)).Returns(() => ArticleWithAttachmentsAndCategories);
            articlesRepositoryMock.Setup(repository => repository.GetPublishedArticleById(It.Is<int>(i => i != 1))).Returns(() => null);

            articlesRepositoryMock.Setup(repository => repository.GetCategoriesWithArticles()).Returns(() => Categories.Where(c => c.Articles.Any()).ToList());
            articlesRepositoryMock.Setup(repository => repository.GetAuthorsWithArticles()).Returns(() => Authors.Where(a => a.Articles.Any()).ToList());

            articlesRepositoryMock.Setup(repository => repository.GetAuthorById(It.IsAny<int>())).Returns<int>(i => Authors.FirstOrDefault(a => a.Id == i));
            articlesRepositoryMock.Setup(repository => repository.GetCategoryById(It.IsAny<int>())).Returns<int>(i => Categories.FirstOrDefault(a => a.Id == i));

            articlesRepositoryMock.Setup(repository => repository.SearchArticleByNamePattern(It.IsAny<string>())).Returns<string>(s => Articles.Where(a => a.DisplayName != null && a.DisplayName.Contains(s)).AsQueryable());
            articlesRepositoryMock.Setup(repository => repository.SearchArticleByDescriptionPattern(It.IsAny<string>())).Returns<string>(s => Articles.Where(a => a.DocumentDescription != null && a.DocumentDescription.Contains(s)).ToList());
            articlesRepositoryMock.Setup(repository => repository.SearchArticleByCategoryNamePattern(It.IsAny<string>())).Returns<string>(s => Categories.Where(c => c.DisplayName != null && c.DisplayName.Contains(s)).ToList());
            articlesRepositoryMock.Setup(repository => repository.SearchArticleByAuthorNamePattern(It.IsAny<string>())).Returns<string>(s => Authors.Where(a => a.Surname != null && a.Name != null && (a.Surname.Contains(s) || a.Name.Contains(s))).ToList());
            
            service = new ReadArticlesService(articlesRepositoryMock.Object);
        }

        [TestMethod]
        public void TestGetAllArticlesModel()
        {
            // Zero page with no filter
            ShowAllArticlesModel model = service.GetAllArticlesModel(null, null, 5, ShowAllArticlesModel.SortingType.ByTitle);
            VerifyShowAllArticlesModel(model, 1, GetArticlesInfos(0, 1, 2, 3, 4));

            // Zero page with filter
            model = service.GetAllArticlesModel(someArticleNameStartPrefix, 0, 5, ShowAllArticlesModel.SortingType.ByTitle);
            VerifyShowAllArticlesModel(model, 1, GetArticlesInfos(0, 1, 6), 3, someArticleNameStartPrefix);

            // Zero page with filter. Last changed date
            model = service.GetAllArticlesModel(someArticleNameStartPrefix, 0, 5, ShowAllArticlesModel.SortingType.ByModificationDate);
            VerifyShowAllArticlesModel(model, 1, GetArticlesInfos(0, 1, 6).OrderByDescending(a => a.LastChangedDate).ToList(), 3, someArticleNameStartPrefix);

            // Zero page with filter. Created date
            model = service.GetAllArticlesModel(someArticleNameStartPrefix, 0, 5, ShowAllArticlesModel.SortingType.ByCreationDate);
            VerifyShowAllArticlesModel(model, 1, GetArticlesInfos(0, 1, 6).OrderByDescending(a => a.CreatedDate).ToList(), 3, someArticleNameStartPrefix);

            // Second page with no filter
            model = service.GetAllArticlesModel(string.Empty, 1, 5, ShowAllArticlesModel.SortingType.ByTitle);
            VerifyShowAllArticlesModel(model, 2, GetArticlesInfos(5, 6, 7), startsFilter:string.Empty);

            // Second page with no filter. Last changed date
            model = service.GetAllArticlesModel(string.Empty, 1, 5, ShowAllArticlesModel.SortingType.ByModificationDate);
            VerifyShowAllArticlesModel(model, 2, GetArticlesInfos(0, 3, 4).OrderByDescending(a => a.LastChangedDate).ToList(), startsFilter: string.Empty);

            // Second page with no filter. Created date
            model = service.GetAllArticlesModel(string.Empty, 1, 5, ShowAllArticlesModel.SortingType.ByCreationDate);
            VerifyShowAllArticlesModel(model, 2, GetArticlesInfos(0, 1, 7).OrderByDescending(a => a.CreatedDate).ToList(), startsFilter: string.Empty);
            
            // Third page with filter
            model = service.GetAllArticlesModel(someArticleNameStartPrefix, 2, 1, ShowAllArticlesModel.SortingType.ByTitle);
            VerifyShowAllArticlesModel(model, 3, GetArticlesInfos(6), 3, someArticleNameStartPrefix);

            // Verify last page data
            model = service.GetAllArticlesModel(null, 7, 1, ShowAllArticlesModel.SortingType.ByTitle);
            VerifyShowAllArticlesModel(model, 8, GetArticlesInfos(7));
            
            // Empty page
            model = service.GetAllArticlesModel(null, 9, 5, ShowAllArticlesModel.SortingType.ByTitle);
            VerifyShowAllArticlesModel(model, 10, GetArticlesInfos()); 
        }

        [TestMethod]
        public void TestGetViewArticleModelById()
        {
            // Check when model is not found
            CheckViewArticleModel(service.GetViewArticleModelById(15), new Article());
            CheckViewArticleModel(service.GetViewArticleModelById(1), ArticleWithAttachmentsAndCategories);
        }

        [TestMethod]
        public void TestGetAuthorsWithArticles()
        {
            AllAuthorsModel model = service.GetAuthorsWithArticles();

            Assert.AreEqual(model.AllAuthors.Count, 2);

            foreach (AuthorInfoModel infoModel in model.AllAuthors)
            {
                Author author = Authors.First(a => a.Id == infoModel.AuthorId);

                Assert.AreEqual(author.Articles.Count, infoModel.ArticlesCount);
                Assert.AreEqual(author.Degree, infoModel.AuthorDegree);
                Assert.AreEqual(ServicesHelper.GetAuthorFullName(author), infoModel.FullName);
            }
        }

        [TestMethod]
        public void TestGetCategoriessWithArticles()
        {
            AllCategoriesModel model = service.GetCategoriesWithArticles();

            Assert.AreEqual(model.AllCategories.Count, 2);

            foreach (ArticleCategoryInfoModel infoModel in model.AllCategories)
            {
                ArticleCategory category = Categories.First(c => c.Id == infoModel.CategoryId);

                Assert.AreEqual(category.Articles.Count, infoModel.CategoryArticlesCount);
                Assert.AreEqual(category.CategoryDescription, infoModel.Description);
                Assert.AreEqual(category.DisplayName, infoModel.DisplayName);
            }
        }

        [TestMethod]
        public void TestGetAuthorArticlesList()
        {
            ArticlesListModel model = service.GetAuthorArticlesList(1);

            Author author = Authors.First(a => a.Id == 1);
            Assert.AreEqual(model.HeaderText, ServicesHelper.GetAuthorFullName(author));
            VerifyArticleInfos(model.ArticlesInfos, author.Articles.ToArray());
        }

        [TestMethod]
        public void GetCategoriesArticlesList()
        {
            ArticlesListModel model = service.GetCategoryArticlesList(1);

            ArticleCategory category = Categories.First(a => a.Id == 1);
            Assert.AreEqual(model.HeaderText, string.Format(MedSiteStrings.AllArtilcesOfCategory, category.DisplayName));
            VerifyArticleInfos(model.ArticlesInfos, category.Articles.ToArray());
        }

        [TestMethod]
        public void GetQuickSearchResultsTest()
        {
            ContentQuickSearchInputModel input = new ContentQuickSearchInputModel { MaxItemsCount = 1, Pattern = "Not existing pattern" };

            // Nothing found test
            IList<ContentQuickSearchItemModel> results = service.GetQuickSearchResults(input, GetArticleUrl, GetCategoryUrl, GetAuthorUrl);
            Assert.IsFalse(results.Any());

            // One Article by name found test
            input.MaxItemsCount = 1;
            input.Pattern = someArticleNameStartPrefix;
            results = service.GetQuickSearchResults(input, GetArticleUrl, GetCategoryUrl, GetAuthorUrl);
            Assert.AreEqual(results.Count, 1);
            VerifyQuickSearchResult(results[0], 0, Articles[0].DocumentDescription, string.Format(ContentQuickSearchInputModel.GetArticleSearchName(), Articles[0].DisplayName), GetArticleUrl(Articles[0].Id));


            // Two articles by name and one by description
            input.MaxItemsCount = 4;
            input.Pattern = someArticleNameStartPrefix;
            results = service.GetQuickSearchResults(input, GetArticleUrl, GetCategoryUrl, GetAuthorUrl);
            Assert.AreEqual(results.Count, 4);
            VerifyQuickSearchResult(results[0], 0, Articles[0].DocumentDescription, string.Format(ContentQuickSearchInputModel.GetArticleSearchName(), Articles[0].DisplayName), GetArticleUrl(Articles[0].Id));
            VerifyQuickSearchResult(results[1], 1, Articles[1].DocumentDescription, string.Format(ContentQuickSearchInputModel.GetArticleSearchName(), Articles[1].DisplayName), GetArticleUrl(Articles[1].Id));
            VerifyQuickSearchResult(results[2], 2, Articles[6].DocumentDescription, string.Format(ContentQuickSearchInputModel.GetArticleSearchName(), Articles[6].DisplayName), GetArticleUrl(Articles[6].Id));
            VerifyQuickSearchResult(results[3], 3, Articles[2].DocumentDescription, string.Format(ContentQuickSearchInputModel.GetArticleSearchName(), Articles[2].DisplayName), GetArticleUrl(Articles[2].Id));

            // 2 categories found
            input.MaxItemsCount = 4;
            input.Pattern = "Cat";
            results = service.GetQuickSearchResults(input, GetArticleUrl, GetCategoryUrl, GetAuthorUrl);
            Assert.AreEqual(results.Count, 2);
            VerifyQuickSearchResult(results[0], 0, string.Empty, string.Format(ContentQuickSearchInputModel.GetCategorySearchName(), Categories[0].DisplayName), GetCategoryUrl(Categories[0].Id));
            VerifyQuickSearchResult(results[1], 1, string.Empty, string.Format(ContentQuickSearchInputModel.GetCategorySearchName(), Categories[1].DisplayName), GetCategoryUrl(Categories[1].Id));

            // 3 authors found
            input.MaxItemsCount = 4;
            input.Pattern = "Name1";
            results = service.GetQuickSearchResults(input, GetArticleUrl, GetCategoryUrl, GetAuthorUrl);
            Assert.AreEqual(results.Count, 3);
            VerifyQuickSearchResult(results[0], 0, string.Empty, string.Format(ContentQuickSearchInputModel.GetAuthorSearchName(), ServicesHelper.GetAuthorFullName(Authors[0])), GetAuthorUrl(Authors[0].Id));
            VerifyQuickSearchResult(results[1], 1, string.Empty, string.Format(ContentQuickSearchInputModel.GetAuthorSearchName(), ServicesHelper.GetAuthorFullName(Authors[1])), GetAuthorUrl(Authors[1].Id));
            VerifyQuickSearchResult(results[2], 2, string.Empty, string.Format(ContentQuickSearchInputModel.GetAuthorSearchName(), ServicesHelper.GetAuthorFullName(Authors[2])), GetAuthorUrl(Authors[2].Id));
        }

        private void VerifyQuickSearchResult(ContentQuickSearchItemModel model, int index, string description, string name, string url)
        {
            Assert.AreEqual(model.Index, index);
            Assert.AreEqual(model.ItemDescription, description ?? string.Empty);
            Assert.AreEqual(model.ItemName, name ?? string.Empty);
            Assert.AreEqual(model.ItemUrl, url);
        }

        private string GetArticleUrl(int id)
        {
            return string.Format("articleUrl/{0}", id);
        }

        private string GetCategoryUrl(int id)
        {
            return string.Format("categoryUrl/{0}", id);
        }

        private string GetAuthorUrl(int id)
        {
            return string.Format("authorUrl/{0}", id);
        }

        private void CheckViewArticleModel(ViewArticleModel model, Article article)
        {
            Assert.IsNotNull(model);
            Assert.AreEqual(model.Content, article.Content);
            Assert.AreEqual(model.DisplayName, article.DisplayName);
            Assert.AreEqual(model.DocumentDescription, article.DocumentDescription);
            Assert.AreEqual(model.Id, article.Id);
            Assert.AreEqual(model.LastChangedDate, article.LastChangedDate);
            Assert.AreEqual(model.Published, article.Published);

            Assert.AreEqual(model.ArticleAttachments.Count, article.Attachments.Count);
            Assert.AreEqual(model.ArticleAuthors.Count, article.Authors.Count);
            Assert.AreEqual(model.ArticleCategories.Count, article.Categories.Count);

            foreach (DocAttachment docAttachment in article.Attachments)
            {
                Assert.AreEqual(model.ArticleAttachments.Count(a => a.AttachmentDisplayName == docAttachment.DisplayName &&
                                                                    a.AttachmentUrl == docAttachment.AttachmentURL && 
                                                                    a.AttachmentType == docAttachment.AttachmentType), 1);
            }

            foreach (AttachmentsModel articleAttachment in model.ArticleAttachments)
            {
                string fullName = articleAttachment.AttachmentType == null
                                      ? articleAttachment.AttachmentDisplayName
                                      : articleAttachment.AttachmentDisplayName + articleAttachment.AttachmentType;
                Assert.IsTrue(articleAttachment.GetFullFileName().Equals(fullName));
            }

            foreach (Author author in article.Authors)
            {
                Assert.AreEqual(model.ArticleAuthors.Count(a => a.AuthorId == author.Id && 
                    a.FullAuthorName == ServicesHelper.GetAuthorFullName(author)), 1);
            }

            foreach (ArticleCategory articleCategory in article.Categories)
            {
                Assert.AreEqual(model.ArticleCategories.Count(c => c.CategoryId == articleCategory.Id &&
                    c.CategoryDisplayName == articleCategory.DisplayName), 1);                
            }
        }

        private IList<ArticleInfo> GetArticlesInfos(params int[] indexes)
        {
            IList<ArticleInfo> infos = new List<ArticleInfo>();
            foreach (int i in indexes)
            {
                ArticleInfo info = new ArticleInfo
                                       {
                                           DisplayName = Articles[i].DisplayName,
                                           DocumentDescription = Articles[i].DocumentDescription,
                                           Id = Articles[i].Id,
                                           LastChangedDate = Articles[i].LastChangedDate,
                                           CreatedDate =  Articles[i].CreatedDate
                                       };
                
                foreach (Author author in Articles[i].Authors)
                    info.AddAuthorName(ServicesHelper.GetAuthorFullName(author));
                
                infos.Add(info);
            }

            return infos;
        }

        private void VerifyShowAllArticlesModel(ShowAllArticlesModel model, int currentPage, IList<ArticleInfo> itemsList, int? totalCount = null, string startsFilter = null)
        {
            Assert.IsNotNull(model);
            Assert.AreEqual(model.CurrentPage, currentPage);

            int totalPagesCount = (int)Math.Ceiling((double)(totalCount ?? Articles.Count) / ShowAllArticlesModel.ItemsPerPage);

            Assert.AreEqual(model.TotalPagesCount, totalPagesCount);
            Assert.AreEqual(model.NameFilter, startsFilter);

            Assert.AreEqual(model.ArticlesInfos.Count, itemsList.Count);

            int index = 0;
            foreach (ArticleInfo info in itemsList)
            {
                ArticleInfo a = model.ArticlesInfos[index];
                ArticleInfo i = info;

                Assert.IsTrue(a.DisplayName == i.DisplayName && a.AuthorsNames == i.AuthorsNames &&
                                                   a.DocumentDescription == i.DocumentDescription && a.Id == i.Id &&
                                                   a.LastChangedDate == i.LastChangedDate);
                
                index++;
            }
        }

        private void VerifyArticleInfos(IList<ArticleInfo> infos, params Article[] artilces)
        {
            Assert.AreEqual(artilces.Length, infos.Count);
            foreach (Article article in artilces)
            {
                ArticleInfo info = new ArticleInfo();
                foreach (Author author in article.Authors)
                    info.AddAuthorName(ServicesHelper.GetAuthorFullName(author));

                
                Assert.AreEqual(
                    infos.Count(a => a.DisplayName == article.DisplayName && a.AuthorsNames == info.AuthorsNames &&
                                                   a.DocumentDescription == article.DocumentDescription && a.Id == article.Id &&
                                                   a.LastChangedDate == article.LastChangedDate), 1);
            }
        }
    }
}
