using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Concrete
{
    public class LogService: ILogService
    {
        private readonly IDocumentsRepository documentsRepository;

        public LogService(IDocumentsRepository newDocumentRepository)
        {
            documentsRepository = newDocumentRepository;
        }

        public BTTAjaxOutputGridModel GetArticleLogData(BTTAjaxInputGridModel inputGridModel)
        {
            return BTTAjaxGridHelper.GetGridData(documentsRepository.Context.ArticleChangesLogs, inputGridModel);
        }
    }
}