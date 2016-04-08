using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Conctrete;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.ControllersServices.Concrete;
using Ninject;
using Ninject.Syntax;

namespace BTTechnologies.MedScience.MVC.Infrastructure.Dependency
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel kernel;

        public NinjectDependencyResolver()
        {
            kernel = new StandardKernel();
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        public IBindingToSyntax<T> Bind<T>()
        {
            return kernel.Bind<T>();
        }

        public IKernel Kernel
        {
            get { return kernel; }
        }

        private void AddBindings()
        {
            // Domain bindings
            kernel.Bind<IAccountsRepository>().To<AccountsRepository>();
            kernel.Bind<IAuthorsRepository>().To<AuthorsRepository>();
            kernel.Bind<IDocumentsRepository>().To<DocumentsRepository>();
            kernel.Bind<IArticlesRepository>().To<ArticlesRepository>();
            kernel.Bind<IReadOnlyDataRepository>().To<ReadOnlyDataRepository>();

            // Controllers services bindings
            kernel.Bind<IAccountService>().To<AccountService>();
            kernel.Bind<IAdminService>().To<AdminService>();
            kernel.Bind<IAuthorService>().To<AuthorService>();
            kernel.Bind<IDocumentService>().To<DocumentService>();
            kernel.Bind<ILogService>().To<LogService>();
            kernel.Bind<IReadArticlesService>().To<ReadArticlesService>();
            kernel.Bind<IMembershipService>().To<MembershipService>();
            kernel.Bind<ISupportService>().To<SupportService>();
            kernel.Bind<IHomeService>().To<HomeService>();
        }
    }
}