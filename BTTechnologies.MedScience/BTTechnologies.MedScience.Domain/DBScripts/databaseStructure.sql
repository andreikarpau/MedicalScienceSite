SET NOCOUNT ON;
USE master;
IF DB_ID('MedScienceDb') IS NULL
	CREATE DATABASE MedScienceDb;
GO

USE MedScienceDb;
GO

/*DROP SECTION*/
IF OBJECT_ID('dbo.AccountActivationCodes', 'U') IS NOT NULL
	DROP TABLE dbo.AccountActivationCodes;

IF OBJECT_ID('dbo.AccountRoles', 'U') IS NOT NULL
	DROP TABLE dbo.AccountRoles;

IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL
	DROP TABLE dbo.Roles;

IF OBJECT_ID('dbo.ArticlesAuthors', 'U') IS NOT NULL
	DROP TABLE dbo.ArticlesAuthors;

IF OBJECT_ID('dbo.ArticlesAttachments', 'U') IS NOT NULL
	DROP TABLE dbo.ArticlesAttachments;

IF OBJECT_ID('dbo.ArticlesCategoriesRelations', 'U') IS NOT NULL
	DROP TABLE dbo.ArticlesCategoriesRelations;		

IF OBJECT_ID('dbo.Articles', 'U') IS NOT NULL
	DROP TABLE dbo.Articles;

IF OBJECT_ID('dbo.Authors', 'U') IS NOT NULL
	DROP TABLE dbo.Authors;

IF OBJECT_ID('dbo.Accounts', 'U') IS NOT NULL
	DROP TABLE dbo.Accounts;

IF OBJECT_ID('dbo.DocAttachments', 'U') IS NOT NULL
	DROP TABLE dbo.DocAttachments;
	
IF OBJECT_ID('dbo.ArticleCategories', 'U') IS NOT NULL
	DROP TABLE dbo.ArticleCategories;	

IF OBJECT_ID('dbo.ArticleChangesLogs', 'U') IS NOT NULL
	DROP TABLE dbo.ArticleChangesLogs;
			
IF OBJECT_ID('dbo.MenuItems', 'U') IS NOT NULL
	DROP TABLE dbo.MenuItems;
			
IF OBJECT_ID('dbo.PageTiles', 'U') IS NOT NULL
	DROP TABLE dbo.PageTiles;
			
IF OBJECT_ID('dbo.SiteFiles', 'U') IS NOT NULL
	DROP TABLE dbo.SiteFiles;

IF OBJECT_ID('dbo.AuthorsFullDataRecords', 'V') IS NOT NULL
	DROP VIEW dbo.AuthorsFullDataRecords;	
	
IF OBJECT_ID('dbo.ConcatAuthors', 'FN') IS NOT NULL
	DROP FUNCTION dbo.ConcatAuthors;

IF OBJECT_ID('dbo.ConcatAttachments', 'FN') IS NOT NULL
	DROP FUNCTION dbo.ConcatAttachments;

IF OBJECT_ID('dbo.ConcatCategories', 'FN') IS NOT NULL
	DROP FUNCTION dbo.ConcatCategories;

IF OBJECT_ID('dbo.ArticlesFullDataRecords', 'V') IS NOT NULL
	DROP VIEW dbo.ArticlesFullDataRecords;	
	
IF OBJECT_ID('dbo.ArticleInsertTrigger', 'TR ') IS NOT NULL
	DROP TRIGGER dbo.ArticleInsertTrigger;	
		
IF OBJECT_ID('dbo.ArticleUpdateTrigger', 'TR') IS NOT NULL
	DROP TRIGGER dbo.ArticleUpdateTrigger;	
		
IF OBJECT_ID('dbo.ArticleDeleteTrigger', 'TR') IS NOT NULL
	DROP TRIGGER dbo.ArticleDeleteTrigger;	
	
IF OBJECT_ID('dbo.DocAttachmentsDeleteTrigger', 'TR ') IS NOT NULL
	DROP TRIGGER dbo.DocAttachmentsDeleteTrigger;	
		
IF OBJECT_ID('dbo.DocAttachmentsInsertTrigger', 'TR') IS NOT NULL
	DROP TRIGGER dbo.DocAttachmentsInsertTrigger;	
		
IF OBJECT_ID('dbo.DocAttachmentsUpdateTrigger', 'TR') IS NOT NULL
	DROP TRIGGER dbo.DocAttachmentsUpdateTrigger;	
			
IF OBJECT_ID('dbo.ArticleChangesLogsInsertTrigger', 'TR') IS NOT NULL
	DROP TRIGGER dbo.ArticleChangesLogsInsertTrigger;	

/*CREATE TABLE SECTION*/

/*Table with account roles*/
CREATE TABLE dbo.Roles (
  Id INT NOT NULL IDENTITY /*Unique Id*/, 
  DisplayName NVARCHAR(128) NOT NULL /*Role display name*/,
  Code NVARCHAR(128) NOT NULL /*Role unique code. It will be used for site permissions*/,
  PRIMARY KEY (id),
  CONSTRAINT roles_name_unique UNIQUE(DisplayName),
  CONSTRAINT roles_сode_unique UNIQUE(Code)
)

/*Table with users data*/
CREATE TABLE dbo.Accounts (
  Id INT NOT NULL IDENTITY /*Unique Id*/, 
  UserLogin NVARCHAR(256) NOT NULL /*Login of the users. Probably it will be user email.*/,
  Name NVARCHAR(128)  /*User name*/,
  Surname NVARCHAR(128) /*User surname*/,
  Patronymic NVARCHAR(128) /*Fathers name*/,
  Phone NVARCHAR(128) /*User phone*/,
  UserPassword NVARCHAR(128) NOT NULL /*User password*/,
  RegistrationDate DATETIME /*Date of user registration*/,
  LastLogInDate DATETIME /*Date of last entering the system*/,
  PRIMARY KEY (Id),
  CONSTRAINT account_userLogin_unique UNIQUE(UserLogin)
);

/*This table determines user account and its role*/
CREATE TABLE dbo.AccountRoles (
  IdAccount INT NOT NULL /*Unique account id*/,
  IdRole INT NOT NULL /*Unique role id*/,
  PRIMARY KEY  (IdAccount, IdRole),
  CONSTRAINT account_role_fk01 FOREIGN KEY(IdAccount) REFERENCES dbo.Accounts(Id),
  CONSTRAINT account_role_fk02 FOREIGN KEY(IdRole) REFERENCES dbo.Roles(Id)
)

/*Table with documents*/
CREATE TABLE dbo.Articles (
  Id INT NOT NULL IDENTITY /*Unique Id*/, 
  DisplayName NVARCHAR(256) NOT NULL /*Document display name*/,
  DocumentDescription NVARCHAR(MAX) /*Document description*/,
  Content NVARCHAR(MAX) /*Document content*/,
  Published BIT NOT NULL CONSTRAINT publiched_default_value DEFAULT (0) /*Shows if article is published*/,
  LastChangedDate DATETIME /*Date when article was last changed*/, 
  PRIMARY KEY (id),
  CONSTRAINT article_name_unique UNIQUE(DisplayName)
)

/*Table with authors*/
CREATE TABLE dbo.Authors (
  Id INT NOT NULL IDENTITY /*Unique Id*/, 
  Name NVARCHAR(128) NOT NULL /*User name*/,
  Surname NVARCHAR(128) NOT NULL /*User surname*/,
  Patronymic NVARCHAR(128) /*Fathers name*/,
  Degree NVARCHAR(1024) /*Scientific degree*/,
  Phone NVARCHAR(128) /*Author phone*/,
  EMail  NVARCHAR(256) /*Author Email*/,
  PhotoLink  NVARCHAR(256) /*Link to author photo*/,
  AuthorStatus BIT /*Author is proved by administrator. 0 - is not proved*/,
  AccountId INT /*Id of linked account. No accounts can be linked.*/
  PRIMARY KEY (id),
  CONSTRAINT author_account_fk01 FOREIGN KEY(AccountId) REFERENCES dbo.Accounts(Id)
)

/*This table determines articles and its' authors*/
CREATE TABLE dbo.ArticlesAuthors (
  IdArticle INT NOT NULL /*Unique article id*/,
  IdAuthor INT NOT NULL /*Unique author id*/,
  PRIMARY KEY  (IdArticle, IdAuthor),
  CONSTRAINT document_author_fk01 FOREIGN KEY(IdArticle) REFERENCES dbo.Articles(Id),
  CONSTRAINT document_author_fk02 FOREIGN KEY(IdAuthor) REFERENCES dbo.Authors(Id)
)

/*Table with authors*/
CREATE TABLE dbo.DocAttachments (
  Id INT NOT NULL IDENTITY /*Unique Id*/, 
  DisplayName NVARCHAR(512) NOT NULL /*Attachment name*/,
  AttachmentURL NVARCHAR(512) /*Attachment URL*/,
  AttachmentType NVARCHAR(128) /*Attachment Type*/,
  DownloadOptions NVARCHAR(128) /*Attachment download options*/
  PRIMARY KEY (id)
)

/*This table determines articles and its' authors*/
CREATE TABLE dbo.ArticlesAttachments (
  IdArticle INT NOT NULL /*Unique article id*/,
  IdDocAttachment INT NOT NULL /*Unique author id*/,
  PRIMARY KEY  (IdArticle, IdDocAttachment),
  CONSTRAINT document_attachment_fk01 FOREIGN KEY(IdArticle) REFERENCES dbo.Articles(Id),
  CONSTRAINT document_attachment_fk02 FOREIGN KEY(IdDocAttachment) REFERENCES dbo.DocAttachments(Id)
)

/*Table with article categories*/
CREATE TABLE dbo.ArticleCategories (
  Id INT NOT NULL IDENTITY /*Unique Id*/, 
  DisplayName NVARCHAR(128) NOT NULL /*Category display name*/,
  CategoryDescription NVARCHAR(256) /*Category description*/,
  PRIMARY KEY (id),
  CONSTRAINT article_categories_name_unique UNIQUE(DisplayName)
)

/*This table determines articles and its' categories*/
CREATE TABLE dbo.ArticlesCategoriesRelations (
  IdArticle INT NOT NULL /*Unique article id*/,
  IdCategory INT NOT NULL /*Unique author id*/,
  PRIMARY KEY  (IdArticle, IdCategory),
  CONSTRAINT article_category_fk01 FOREIGN KEY(IdArticle) REFERENCES dbo.Articles(Id),
  CONSTRAINT article_category_fk02 FOREIGN KEY(IdCategory) REFERENCES dbo.ArticleCategories(Id)
)

/*Table with article changes data*/
CREATE TABLE dbo.ArticleChangesLogs (
  Id BigInt NOT NULL IDENTITY /*Unique Id*/, 
  ItemName NVARCHAR(128) /*Article display name*/,
  ItemId INT /*Article Id*/,
  AccountId INT /*Account that changed the article*/,
  LoginWhoChanged NVARCHAR(256) /*Login if the account which changed the article*/,
  DBUser NVARCHAR(256) /*DB System user*/,
  ChangesInformation NVARCHAR(MAX),
  LogDateTime DATETIME /*Log date time*/, 
  PRIMARY KEY (id)
)
GO

ALTER TABLE dbo.Articles ADD CONSTRAINT articles_changed_date DEFAULT GETDATE() FOR LastChangedDate;
GO

ALTER TABLE dbo.ArticleChangesLogs ADD CONSTRAINT log_date_default DEFAULT GETDATE() FOR LogDateTime;
GO
/*CREATE FUNCTION SECTION*/
CREATE FUNCTION dbo.ConcatAuthors (@articleId AS INT) RETURNS VARCHAR(MAX) 
AS
BEGIN
	DECLARE @names AS VARCHAR(MAX);
	SET @names = '';
	SELECT @names = @names + 
      CASE WHEN LTRIM(@names) = '' THEN 
	  ''
      ELSE
	  ', '
	  END + COALESCE(au.Surname, '') + ' ' + COALESCE(au.Name, '') + ' ' + COALESCE(au.Patronymic, '') 
	FROM  dbo.ArticlesAuthors AS aa LEFT JOIN dbo.Authors AS au ON aa.IdAuthor = au.Id
	WHERE aa.IdArticle = @articleId;
	RETURN @names;
END
GO

CREATE FUNCTION dbo.ConcatAttachments (@articleId AS INT) RETURNS VARCHAR(MAX) 
AS
BEGIN
	DECLARE @names AS VARCHAR(MAX);
	SET @names = '';
	SELECT @names = @names + 
	 CASE WHEN da.DisplayName IS NOT NULL THEN 
	  CASE WHEN LTRIM(@names) = '' THEN 
	  ''
      ELSE
	  '; '
	  END
	  + da.DisplayName + ' ' + COALESCE(da.AttachmentType, '') 
	 ELSE ''
	 END
	FROM  dbo.ArticlesAttachments AS aa LEFT JOIN dbo.DocAttachments AS da ON aa.IdDocAttachment = da.Id
	WHERE aa.IdArticle = @articleId;
	RETURN @names;
END
GO

CREATE FUNCTION dbo.ConcatCategories (@articleId AS INT) RETURNS VARCHAR(MAX) 
AS
BEGIN
	DECLARE @names AS VARCHAR(MAX);
	SET @names = '';
	SELECT @names = @names + 
	  CASE WHEN LTRIM(@names) = '' THEN 
	  ''
      ELSE
	  '; '
	  END + c1.DisplayName 
	FROM  dbo.ArticlesCategoriesRelations AS ac LEFT JOIN dbo.ArticleCategories AS c1 ON ac.IdCategory = c1.Id
	WHERE ac.IdArticle = @articleId;
	RETURN @names;
END
GO

/*CREATE VIEW SECTION*/
CREATE VIEW dbo.AuthorsFullDataRecords
 AS
SELECT Id, Name, Surname, Patronymic, Degree, Phone, EMail, PhotoLink, AuthorStatus,  
 (SELECT UserLogin 
  FROM dbo.Authors AS au2 LEFT JOIN dbo.Accounts AS ar ON au2.AccountId = ar.Id
  WHERE au2.Id = au.Id) AS LinkedLogin, 
 (SELECT COUNT(*)
  FROM dbo.Authors AS au2 LEFT JOIN dbo.ArticlesAuthors AS aa ON au2.Id = aa.IdAuthor LEFT JOIN dbo.Articles AS ar ON aa.IdArticle = ar.Id
  WHERE au2.Id = au.Id) AS ArticlesCount    
FROM dbo.Authors AS au

GO

CREATE VIEW dbo.ArticlesFullDataRecords
AS
SELECT ar.Id, ar.DisplayName, ar.DocumentDescription, dbo.ConcatAuthors(ar.Id) AS AuthorsNames, 
 (SELECT COUNT(id) FROM dbo.ArticlesAttachments AS aa LEFT JOIN dbo.DocAttachments AS da ON aa.IdDocAttachment = da.Id WHERE aa.IdArticle = ar.Id) 
 AS AttachmentsCount, dbo.ConcatCategories(ar.Id) AS Categories, ar.Published, ar.LastChangedDate
FROM dbo.Articles AS ar 
GO

/*CREATE TRIGGERS SECTION*/
CREATE TRIGGER dbo.ArticleInsertTrigger
   ON dbo.Articles
   AFTER INSERT
AS 
BEGIN
	INSERT INTO dbo.ArticleChangesLogs (ItemId, ItemName, ChangesInformation, DBUser)
	SELECT i.Id, i.DisplayName, 'Article inserted' AS ChangesInformation, SYSTEM_USER AS DBUser 
	FROM inserted AS i
	END;
GO

CREATE TRIGGER dbo.ArticleUpdateTrigger
   ON dbo.Articles
   AFTER UPDATE
AS 
BEGIN
	INSERT INTO dbo.ArticleChangesLogs (ItemId, ItemName, ChangesInformation, DBUser)
	SELECT i.Id, d.DisplayName, 'Article updated: ' + 
	 CASE WHEN i.DisplayName != d.DisplayName THEN 'DisplayName old - ' + d.DisplayName + ' new - ' + i.DisplayName + ' ' ELSE '' END + 
	 CASE WHEN i.DocumentDescription != d.DocumentDescription THEN 'DocumentDescription old - ' + d.DocumentDescription + ' new - ' + i.DocumentDescription + ' ' ELSE '' END + 
	 CASE WHEN i.Content != d.Content THEN 'Content changed Old length - ' + CAST(LEN(d.Content) AS VARCHAR) + ' new - ' + CAST(LEN(i.Content) AS VARCHAR) ELSE '' END + 
	 CASE WHEN i.Published != d.Published THEN 'Published old - ' + CAST(d.Published AS VARCHAR) + ' new - ' + CAST(i.Published AS VARCHAR) + ' ' ELSE '' END 
	AS ChangesInformation, SYSTEM_USER AS DBUser 
	FROM inserted AS i JOIN deleted AS d ON i.Id = d.Id
	END;
GO

CREATE TRIGGER dbo.ArticleDeleteTrigger
   ON dbo.Articles
   AFTER INSERT
AS 
BEGIN
	INSERT INTO dbo.ArticleChangesLogs (ItemId, ItemName, ChangesInformation, DBUser)
	SELECT i.Id, i.DisplayName, 'Article deleted' AS ChangesInformation, SYSTEM_USER AS DBUser 
	FROM deleted AS i
	END;
GO

CREATE TRIGGER dbo.DocAttachmentsDeleteTrigger
   ON dbo.DocAttachments
   AFTER DELETE
AS 
BEGIN
	INSERT INTO dbo.ArticleChangesLogs (ItemId, ItemName, ChangesInformation, DBUser)
	SELECT i.Id, i.DisplayName, 'Attachment deleted' AS ChangesInformation, SYSTEM_USER AS DBUser 
	FROM deleted AS i
	END
GO

CREATE TRIGGER dbo.DocAttachmentsInsertTrigger
   ON dbo.DocAttachments
   AFTER INSERT
AS 
BEGIN
	INSERT INTO dbo.ArticleChangesLogs (ItemId, ItemName, ChangesInformation, DBUser)
	SELECT i.Id, i.DisplayName, 'Attachment inserted' AS ChangesInformation, SYSTEM_USER AS DBUser 
	FROM inserted AS i
	END;
GO

CREATE TRIGGER dbo.DocAttachmentsUpdateTrigger
   ON dbo.DocAttachments
   AFTER UPDATE
AS 
BEGIN
	INSERT INTO dbo.ArticleChangesLogs (ItemId, ItemName, ChangesInformation, DBUser)
	SELECT i.Id, i.DisplayName, 'Attachment changed' AS ChangesInformation, SYSTEM_USER AS DBUser 
	FROM inserted AS i
	END;
GO

CREATE TRIGGER dbo.ArticleChangesLogsInsertTrigger
   ON dbo.ArticleChangesLogs
   AFTER INSERT
AS 
BEGIN
	IF (SELECT COUNT(*) FROM dbo.ArticleChangesLogs) > 10000000
	BEGIN
		WITH rowsToDelete AS
		(
			SELECT TOP(3000000) *
			FROM dbo.ArticleChangesLogs
			ORDER BY LogDateTime
		)
		DELETE FROM rowsToDelete
	END
END
GO

/*DB MODIFICATIONS*/
/*This table save user activation codes*/
CREATE TABLE dbo.AccountActivationCodes (
  Id BigInt NOT NULL IDENTITY /*Unique Id*/, 
  IdAccount INT NOT NULL /*User account id*/,
  Code NVARCHAR(128) NOT NULL /*Activation code*/,
  LastChangedDate DATETIME NOT NULL /*Date when activation code was added*/, 
  PRIMARY KEY  (Id),
  CONSTRAINT account_activation_fk01 FOREIGN KEY(IdAccount) REFERENCES dbo.Accounts(Id),
)

/*DB MODIFICATIONS 2*/
/*Changes Articles and ArticleChangesLog display name max sizes to 512*/
ALTER TABLE dbo.Articles
ALTER COLUMN DisplayName NVARCHAR(512)
GO

ALTER TABLE dbo.ArticleChangesLogs
ALTER COLUMN ItemName NVARCHAR(512)
GO

/*DB MODIFICATIONS 3*/
ALTER TABLE dbo.Articles
ADD CreatedDate DATETIME NOT NULL DEFAULT GETDATE() /*Date when article was created*/
GO

UPDATE dbo.Articles
SET CreatedDate = LastChangedDate
GO

/*DB MODIFICATIONS 4*/

/*Table with menus data*/
CREATE TABLE dbo.MenuItems (
  Id INT NOT NULL IDENTITY /*Unique Id*/, 
  MenuType INT NOT NULL /*Type of menu*/,
  DispayName NVARCHAR(256)  /*Menu item display name*/,
  ItemOrder INT /*Order of item in menu list*/,
  ItemUrl NVARCHAR(512) /*Url of meni item*/,
  PRIMARY KEY (Id)
);
GO

/*Table with pages tiles data (especially for main page)*/
CREATE TABLE dbo.PageTiles (
  Id INT NOT NULL IDENTITY /*Unique Id*/, 
  PageType INT NOT NULL /*Type of menu*/,
  TileType INT NOT NULL /*Type of tile*/,
  TileStyles NVARCHAR(128) /*Styles of this tile*/,
  Content NVARCHAR(MAX) /*Tile content (Html possible)*/,
  ItemOrder INT /*Item order*/,
  PRIMARY KEY (Id)
);
GO

/*Table with list of files existed on the site*/
CREATE TABLE dbo.SiteFiles (
  Id INT NOT NULL IDENTITY /*Unique Id*/, 
  DisplayName NVARCHAR(512) NOT NULL /*File name*/,
  FileUrl NVARCHAR(512) /*File URL*/,
  PRIMARY KEY (id)
)
GO


