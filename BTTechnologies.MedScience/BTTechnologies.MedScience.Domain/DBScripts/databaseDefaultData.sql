USE MedScienceDb;
GO

IF OBJECT_ID('dbo.AccountRoles', 'U') IS NOT NULL
	TRUNCATE TABLE dbo.AccountRoles;

IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL
	DELETE FROM dbo.Roles;

IF OBJECT_ID('dbo.ArticlesAuthors', 'U') IS NOT NULL
	TRUNCATE TABLE dbo.ArticlesAuthors;

IF OBJECT_ID('dbo.ArticlesAttachments', 'U') IS NOT NULL
	TRUNCATE TABLE dbo.ArticlesAttachments;

IF OBJECT_ID('dbo.ArticlesCategoriesRelations', 'U') IS NOT NULL
	TRUNCATE TABLE dbo.ArticlesCategoriesRelations;

IF OBJECT_ID('dbo.Articles', 'U') IS NOT NULL
	DELETE FROM dbo.Articles;

IF OBJECT_ID('dbo.Authors', 'U') IS NOT NULL
	DELETE FROM dbo.Authors;

IF OBJECT_ID('dbo.Accounts', 'U') IS NOT NULL
	DELETE FROM dbo.Accounts;

IF OBJECT_ID('dbo.DocAttachments', 'U') IS NOT NULL
	DELETE FROM dbo.DocAttachments;

IF OBJECT_ID('dbo.ArticleCategories', 'U') IS NOT NULL
	DELETE FROM dbo.ArticleCategories;

IF OBJECT_ID('dbo.MenuItems', 'U') IS NOT NULL
	DELETE FROM dbo.MenuItems;
	
IF OBJECT_ID('dbo.PageTiles', 'U') IS NOT NULL
	DELETE FROM dbo.PageTiles;

DECLARE @AccountAdminVar TABLE(AdminId INT);
DECLARE @RoleAdministratorVar TABLE (RoleId INT);

INSERT INTO dbo.Accounts
OUTPUT INSERTED.Id INTO @AccountAdminVar
VALUES ('admin', 'TestAdmin', 'TestAdminSurname', 'TestAdminPatronic', '+37529 1111111', 'admin', '3/18/2013', NULL);

INSERT INTO dbo.Roles
OUTPUT INSERTED.Id INTO @RoleAdministratorVar
VALUES ('Administrator', 'AdminRole')

INSERT INTO dbo.AccountRoles
VALUES ((select AdminId from @AccountAdminVar), (select RoleId from @RoleAdministratorVar))

DELETE @RoleAdministratorVar;

INSERT INTO dbo.Roles
OUTPUT INSERTED.Id INTO @RoleAdministratorVar
VALUES ('Super Administrator', 'SuperAdminRole')

INSERT INTO dbo.AccountRoles
VALUES ((select AdminId from @AccountAdminVar), (select RoleId from @RoleAdministratorVar))
GO

INSERT INTO dbo.Roles
VALUES ('User', 'UserRole'), ('Not Activated User', 'NotActivatedUserRole'), ('Author Role', 'AuthorRole');
GO

/*Temp data*/
DECLARE @AuthorsIdsVar TABLE (AuthorId INT);
DECLARE @ArticlesIdsVar TABLE (ArticleId INT);

INSERT INTO dbo.Authors (Name, Surname, Patronymic, Degree, Phone, EMail, PhotoLink, AuthorStatus)
OUTPUT INSERTED.Id INTO @AuthorsIdsVar
VALUES 
 ('Vasiliy', 'Pupkin', 'Vitalievich', 'Docent', '11100929293', 'pupkin@gmail.com', NULL, '0'), 
 ('Peter', 'Vasin', 'Anatolievich', 'Proffesor', '123123123123', 'peter@gmail.com', NULL, '1'),
 ('Eugen', 'Ivanov', 'Ignatievich', 'Student', '23332323232', 'eugen@gmail.com', NULL, '0') 

INSERT INTO dbo.Articles(DisplayName, DocumentDescription, Content, Published)
OUTPUT INSERTED.Id INTO @ArticlesIdsVar
VALUES 
 ('About Cardiology', 'description1 about cardiology', 'Some interesting content about cardiology', 1), 
 ('About flue', 'description2 about flu', 'Some interesting content about flue', 1), 
 ('About staphilococus', 'description3 about staphilococus', 'Some interesting content about staphilococus!!!', 0)

INSERT INTO dbo.ArticlesAuthors (IdArticle, IdAuthor)
SELECT ArticleId, AuthorId 
FROM @AuthorsIdsVar AS a CROSS JOIN @ArticlesIdsVar
WHERE a.AuthorId != (SELECT id FROM dbo.Authors WHERE Surname = 'Ivanov')

DELETE FROM dbo.ArticlesAuthors
WHERE IdArticle = (SELECT id FROM dbo.Articles WHERE DisplayName = 'About staphilococus') AND IdAuthor = (SELECT id FROM dbo.Authors WHERE Surname = 'Pupkin')

DECLARE @AccountVar TABLE(AccountId INT);
DECLARE @RoleAdministratorVar TABLE (RoleId INT);

INSERT INTO dbo.Accounts
OUTPUT INSERTED.Id INTO @AccountVar
VALUES ('pupkin@gmail.com', 'Vasiliy', 'Pupkin', 'Vitalievich', '+37529 11100929293', '11111', '6/18/2013', NULL);

INSERT INTO dbo.AccountRoles
VALUES ((SELECT AccountId FROM @AccountVar), (SELECT id FROM dbo.Roles WHERE code = 'UserRole')), ((SELECT AccountId FROM @AccountVar), (SELECT id FROM dbo.Roles WHERE code = 'AuthorRole'))

UPDATE dbo.Authors
SET AccountId = (SELECT id FROM dbo.Accounts WHERE UserLogin = 'pupkin@gmail.com')
WHERE Surname = 'Pupkin'
GO

INSERT INTO dbo.DocAttachments
VALUES ('History of cardiology', 'medscience.ru/hof', 'pdf', 'All'), ('Some words about cardiology in ancient Greece', 'medscience.ru/hinGreece', 'doc', 'All'), 
('Flue 2013', 'medscience.ru/flue2013', 'pdf', 'Registered')
GO

INSERT INTO dbo.ArticlesAttachments
SELECT (SELECT Id FROM dbo.Articles WHERE DisplayName = 'About Cardiology') AS IdArticle, da.Id AS IdDocAttachment
FROM dbo.DocAttachments AS da
WHERE da.DisplayName IN ('History of cardiology', 'Some words about cardiology in ancient Greece')
GO

INSERT INTO dbo.ArticlesAttachments
SELECT (SELECT Id FROM dbo.Articles WHERE DisplayName = 'About flue') AS IdArticle, da.Id AS IdDocAttachment
FROM dbo.DocAttachments AS da
WHERE da.DisplayName = 'Flue 2013'
GO

INSERT INTO dbo.ArticleCategories
VALUES ('Infection deseases', 'Description about infection deseases'), ('Infections', 'These all is infections'), ('Cardiology', 'Heart deseases'), ('Hypotitys', 'Infection desease type')
GO

INSERT INTO dbo.ArticlesCategoriesRelations
SELECT ar.Id AS IdArticle, (SELECT Id FROM dbo.ArticleCategories WHERE DisplayName = 'Infection deseases') AS IdCategory
FROM dbo.Articles AS ar
WHERE ar.DisplayName = 'About flue' OR ar.DisplayName = 'About staphilococus'
GO

INSERT INTO dbo.ArticlesCategoriesRelations
SELECT ar.Id AS IdArticle, (SELECT Id FROM dbo.ArticleCategories WHERE DisplayName = 'Infections') AS IdCategory
FROM dbo.Articles AS ar
WHERE ar.DisplayName = 'About staphilococus'
GO

INSERT INTO dbo.ArticlesCategoriesRelations
SELECT ar.Id AS IdArticle, (SELECT Id FROM dbo.ArticleCategories WHERE DisplayName = 'Cardiology') AS IdCategory
FROM dbo.Articles AS ar
WHERE ar.DisplayName = 'About Cardiology' 
GO

INSERT INTO dbo.PageTiles
VALUES (1, 1, 'big-content-container', '<p><span style="font-size:26px">Сайт находится в разработке!</span></p>', 0),
(1, 1, 'small-content-container', '', 1),
(1, 1, 'small-content-container', '', 2),
(1, 1, 'small-content-container', '', 3),
(1, 2, 'middle-content-container', '<p><span style="color:#395475"><span style="font-size:24px">Новое на сайте</span></span></p><p>&nbsp;</p>', 4),
(1, 1, 'small-content-container', '', 5),
(2, 1, 'big-content-container', '<p style="text-align:center"><strong><span style="font-size:28px"><span style="color:#395475">О сайте</span></span></strong></p>', 0)
GO


/*Insert 256 empty articles*/
 WITH
  L0   AS(SELECT 1 AS c UNION ALL SELECT 1),
  L1   AS(SELECT 1 AS c FROM L0 AS A CROSS JOIN L0 AS B),
  L2   AS(SELECT 1 AS c FROM L1 AS A CROSS JOIN L1 AS B),
  L3   AS(SELECT 1 AS c FROM L2 AS A CROSS JOIN L2 AS B),
  Nums(n) AS (SELECT ROW_NUMBER() OVER(ORDER BY (SELECT 1)) FROM L3)
 INSERT INTO dbo.Articles(DisplayName, DocumentDescription, Published)
 SELECT 'Article' + CAST(n AS VARCHAR), 'Description' + CAST(n AS VARCHAR), 1
 FROM Nums

SELECT * FROM dbo.Accounts
SELECT * FROM dbo.Authors
SELECT * FROM dbo.Articles
SELECT * FROM dbo.ArticlesAuthors
SELECT * FROM dbo.DocAttachments
SELECT * FROM dbo.ArticlesAttachments
SELECT * FROM dbo.ArticleCategories
SELECT * FROM dbo.ArticlesCategoriesRelations

SELECT * FROM dbo.AuthorsFullDataRecords
SELECT * FROM dbo.ArticlesFullDataRecords
SELECT * FROM dbo.PageTiles