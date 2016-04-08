using System;
using System.Collections.Generic;

namespace BTTechnologies.MedScience.MVC.BTTClasses
{
    public class BTTSimplePagerModel
    {
        public const int PagesNumbersCountFromEachSideDefault = 3;

        public BTTSimplePagerModel(int currentPage, int totalPagesCount, Func<int, string> getActionUrlFunction)
        {
            CurrentPage = currentPage;
            GetActionUrlFunction = getActionUrlFunction;
            TotalPagesCount = totalPagesCount;
            PagesNumbersCountFromEachSide = PagesNumbersCountFromEachSideDefault;
        }

        public Func<int, string> GetActionUrlFunction { get; private set; }
        public int CurrentPage { get; private set; }
        public int TotalPagesCount { get; private set; }
        public int PagesNumbersCountFromEachSide { get; private set; }

        public IDictionary<int, string> GetPagesNumbers(out int? firstPage, out int? lastPage)
        {
            firstPage = null;
            lastPage = null;

            IDictionary<int, string> numsList = new Dictionary<int, string>();

            if (GetActionUrlFunction == null || CurrentPage <= 0 || TotalPagesCount < CurrentPage || TotalPagesCount < 1 || PagesNumbersCountFromEachSide < 1)
                return numsList;


            int leftIndex = CurrentPage <= PagesNumbersCountFromEachSide ? 1 : CurrentPage - PagesNumbersCountFromEachSide;
            int rightIndex = TotalPagesCount < CurrentPage + PagesNumbersCountFromEachSide ? TotalPagesCount : CurrentPage + PagesNumbersCountFromEachSide;

            for (int i = leftIndex; i <= rightIndex; i++)
            {
                numsList.Add(i, GetActionUrlFunction(i));
            }

            if (1 < leftIndex)
                firstPage = 1;

            if (rightIndex < TotalPagesCount)
                lastPage = TotalPagesCount;

            return numsList;
        }
    }
}