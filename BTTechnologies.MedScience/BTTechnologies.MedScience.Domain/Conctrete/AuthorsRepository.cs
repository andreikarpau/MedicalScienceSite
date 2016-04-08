using System.Collections.Generic;
using System.Data;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;
using System.Linq;

namespace BTTechnologies.MedScience.Domain.Conctrete
{
    /// <summary>
    /// Authors repository
    /// </summary>
    public class AuthorsRepository : BaseRepository, IAuthorsRepository 
    {
        /// <summary>
        /// <see cref="IAuthorsRepository.GetLinkedAccountNameById"/>
        /// </summary>
        public string GetLinkedAccountNameById(int id)
        {
            Author author = Context.Authors.FirstOrDefault(a => a.Id == id);
            if (author == null || author.Account == null)
                return string.Empty;

            return author.Account.UserLogin;
        }

        /// <summary>
        /// <see cref="IAuthorsRepository.GetArticlesCountById"/>
        /// </summary>
        public int GetArticlesCountById(int id)
        {
            Author author = Context.Authors.FirstOrDefault(a => a.Id == id);
            if (author == null || author.Articles == null)
                return 0;

            return author.Articles.Count;
        }

        /// <summary>
        /// <see cref="IAuthorsRepository.GetAccountsWithoutAuthors"/>
        /// </summary>
        public IList<Account> GetAccountsWithoutAuthors()
        {
            return Context.Accounts.Include("Authors").Where(a => !a.Authors.Any()).ToList();
        }

        /// <summary>
        /// <see cref="IAuthorsRepository.GetAccountByAuthorId"/>
        /// </summary>
        public Account GetAccountByAuthorId(int id)
        {
            Author author = Context.Authors.FirstOrDefault(a => a.Id == id);
            return author != null ? author.Account : null;
        }

        /// <summary>
        /// <see cref="IAuthorsRepository.AddOrUpdateAuthor"/>
        /// </summary>
        public void AddOrUpdateAuthor(Author author)
        {
            Account account = null;
            if (author.AccountId != null)
                account = Context.Accounts.FirstOrDefault(a => a.Id == author.AccountId);

            if (author.Id == 0)
            {
                if (account != null)
                    author.Account = account;

                Context.Authors.Add(author);
            }
            else
            {
                if (account == null)
                    author.AccountId = null;

                author.Account = account;
                Context.Entry(author).State = EntityState.Modified;
            }

            Role authorRole = Context.Roles.First(r => r.Code == Role.AUTHOR_ROLE_CODE);
            if (account != null && !account.Roles.Contains(authorRole))
            {
                account.Roles.Add(authorRole);
                Context.Entry(account).State = EntityState.Modified;
            }
            CheckClearAllAuthorsRolesWithoutSave();
            Context.SaveChanges();
        }

        /// <summary>
        /// <see cref="IAuthorsRepository.RemoveAuthor"/>
        /// </summary>
        public void RemoveAuthor(Author author)
        {
            author.Account = null;
            author.AccountId = null;
            author.Articles.Clear();
            Context.Authors.Remove(author);
            CheckClearAllAuthorsRolesWithoutSave();
            Context.SaveChanges();
        }

        /// <summary>
        /// <see cref="IAuthorsRepository.GetAuthorByNameSurnamePatronymicPart"/>
        /// </summary>
        public IList<Author> GetAuthorByNameSurnamePatronymicPart(string namePart)
        {
            return Context.Authors.Where(a => a.Name.Contains(namePart) || a.Surname.Contains(namePart) || a.Patronymic.Contains(namePart)).ToList();
        }

        private void CheckClearAllAuthorsRolesWithoutSave()
        {
            Role authorRole = Context.Roles.First(r => r.Code == Role.AUTHOR_ROLE_CODE);
            if (authorRole.Accounts == null)
                return;

            List<Account> needToDeleteAccounts = authorRole.Accounts.Where(a => a.Authors == null || !a.Authors.Any()).ToList();
            foreach (Account account in needToDeleteAccounts)
            {
                account.Roles.Remove(authorRole);
                Context.Entry(account).State = EntityState.Modified;
            }
        }
    }
}
