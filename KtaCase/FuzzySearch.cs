using Agility.Sdk.Model;
using Agility.Sdk.Model.Capture;
using Agility.Sdk.Model.Forms.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalAgility.Sdk;

namespace KtaCase
{
    public class FuzzySearch
    {
        public FuzzyRecordDisplay[] Search(string sessionId, string searchText, string dbName)
        {
            // ideally this should match the separation characters defined in the fuzzy database config
            var separatorChars = new char[] { ' ', ',', '-' };

            var dbId = new FuzzyDatabaseIdentity()
            {
                Name = "ASD",
                DisplayName= "ASD"
            };

            //var filter= new FuzzySearchFilter()
            //{
            //    FieldName
            //}

            var filter = new FuzzySearchFilter()
            {
                FieldName = "PRESIDENT",
                FilterText = "James Madisin"
            };

            var filters = new FuzzySearchFilterCollection();
            filters.Add(filter);

            var query = new SearchQuery()
            {
                SearchWords = searchText.Split(separatorChars),
                NumberOfRowsToReturn=10,
                FuzzySearchFilters=filters
            };

            var dt = new DocumentTypeIdentity()
            {
                Id = "954E417148294312B4FE6452BE25FE48"
            };

            var ds = new CaptureDocumentService();

            var result = ds.SearchFuzzyDatabase(sessionId, dbId, query, dt);

            return FuzzyRecordDisplay.FromSearchResult(result);
        }
        
        
    }



    public class FuzzyRecordDisplay
    {
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }
        public string Column7 { get; set; }


        public FuzzyRecordDisplay()
        {

        }

        public FuzzyRecordDisplay(FuzzyRecordItem fuzzyRecordItem)
        {
            for (int i = 0; i < fuzzyRecordItem.Columns.Length; i++)
            {
                SetPropertyByName("Column" + (i + 1).ToString(), fuzzyRecordItem.Columns[i]);
            }
        }

        private void SetPropertyByName(string name, string value)
        {
            var p = GetType().GetProperty(name);
            if (p != null)
            {
                p.SetValue(this, (object)value);
            }

            // no op for non existant field
        }

        public static FuzzyRecordDisplay[] FromSearchResult(SearchResult searchResult)
        {
            return searchResult.RecordItems.Select(record => new FuzzyRecordDisplay(record)).ToArray();
        }
    }
}
