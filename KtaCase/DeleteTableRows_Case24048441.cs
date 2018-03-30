using Agility.Sdk.Model.Capture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalAgility.Sdk;

// To find IIS process to debug: %windir%\system32\inetsrv\appcmd.exe list wp

namespace KtaCase
{
    public class DeleteTableRows_Case24048441
    {
        public void DeleteAllRows(string sessionId, string docId, string fieldName)
        {
            var ds = new CaptureDocumentService();

            // Get row count
            var tableValues=ds.GetDocumentTableFieldValue(sessionId, null, docId, new TableFieldIdentity()
            {
                //Id = tableId
                Name=fieldName
            });
            var rowCount = tableValues.Tables[0].Rows.Count;

            // Populate a collection with all indexes
            var rowIndexes = new RowIndexCollection();

            for (int row=0; row<rowCount; row++)
            {
                rowIndexes.Add(new RowIndex()
                {
                    Index = row
                });
            }

            ds.DeleteTableFieldRow(sessionId, null, docId, fieldName, rowIndexes);
        }
    }
}
