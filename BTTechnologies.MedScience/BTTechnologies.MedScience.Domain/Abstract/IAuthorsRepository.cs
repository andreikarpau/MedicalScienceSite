using System.Collections.Generic;
using BTTechnologies.MedScience.Domain.Context;
using BTTechnologies.MedScience.Domain.Entities;

namespace BTTechnologies.MedScience.Domain.Abstract
{
    /// <summary>
    /// Authors repository 
    /// </summary>
    public interface IAuthorsRepository
    {
        /// <summary>
        /// Table context
        /// </summary>
        MedScienceDBContext Context { get; }

        /// <summary>
        /// Get linked account name
        /// </summary>
        string GetLinkedAccountNameById(int id);

        /// <summary>
        /// Author's articles count
        /// </summary>
        int GetArticlesCountById(int id);

        /// <summary>
        /// Accounts with no author
        /// </summary>
        IList<Account> GetAccountsWithoutAuthors();

        /// <summary>
        /// Get linked Account by author id
        /// </summary>
        Account GetAccountByAuthorId(int id);

        /// <summary>
        /// Add or update author 
        /// </summary>
        void AddOrUpdateAuthor(Author author);

        /// <summary>
        /// Get author by id
        /// </summary>
        Author GetAuthorById(int id);

        /// <summary>
        /// Remove author
        /// </summary>
        void RemoveAuthor(Author author);

        /// <summary>
        /// Get authors whose name, surname or patronymic contains namePart
        /// </summary>
        IList<Author> GetAuthorByNameSurnamePatronymicPart(string namePart);
    }
}
