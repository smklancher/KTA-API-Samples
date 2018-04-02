using Agility.Sdk.Model.Capture;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TotalAgility.Sdk;

namespace KtaCase
{
    public class SaveSourceFile_Case24062565
    {
        public string SaveSourceFile(string sessionId, string documentId, string folderPath)
        {
            Debugger.Launch();

            var ds = new CaptureDocumentService();

            string OriginalFileName = "UnknownOriginalName-" + documentId;

            try
            {
                OriginalFileName = ds.GetTextExtension(sessionId, null,documentId, "Kofax.CEBPM.FileName");
            }
            catch (Exception ex)
            {
                Debug.Print($"Error getting original name: {ex.Message}");
            }

            var filePath = Path.Combine(folderPath, OriginalFileName);
            Debug.Print($"Writing source file to path: {filePath}");

            DocumentSourceFile sourceFile = null;

            try
            {
                // Get the source file which will not exist if the source was tif
                sourceFile = ds.GetSourceFile(sessionId, null, documentId);
            }
            catch (Exception ex)
            {
                Debug.Print($"Could not get source file: {ex.Message}");
            }

            if (sourceFile!=null && sourceFile.SourceFile!=null && sourceFile.SourceFile.Length>0)
            {
                Debug.Print($"Source file mime type: {sourceFile.MimeType}");

                File.WriteAllBytes(filePath, sourceFile.SourceFile);
            }
            else
            {
                // If no source file, get doc file as multipage tif
                filePath=ds.GetDocumentAsFile(sessionId, documentId, folderPath, OriginalFileName);
            }

            return filePath;
        }

        public string[] SaveSourceFiles(string sessionId, string folderId, string folderPath)
        {
            var ds = new CaptureDocumentService();
            var folder = ds.GetFolder(sessionId, null, folderId);
            var files = new List<string>();

            foreach (var doc in folder.Documents)
            {
                files.Add(SaveSourceFile(sessionId, doc.Id, folderPath));
            }

            return files.ToArray<string>();
        }
    }
}
