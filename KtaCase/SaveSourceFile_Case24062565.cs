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
        /// <summary>
        /// This tries to save the source file of a document to the given folder, using the original file name stored in 
        /// the text extension named "Kofax.CEBPM.FileName" if it exists.  If the text extension does not exist, then the filename is 
        /// "UnknownOriginalName-" + documentId (no file extension).
        /// If the input files for the document were tiffs, then no source file exists and a multi-page tiff is saved (with extension tiff).
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="documentId"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public string SaveSourceFile(string sessionId, string documentId, string folderPath)
        {
            string OriginalFileName = OriginalFilenameOrDocId(sessionId, documentId);

            var filePath = Path.Combine(folderPath, OriginalFileName);
            Debug.Print($"Writing source file to path: {filePath}");

            DocumentSourceFile sourceFile = null;

            var ds = new CaptureDocumentService();
            try
            {
                // Get the source file which will not exist if the source was tif
                sourceFile = ds.GetSourceFile(sessionId, null, documentId);
            }
            catch (Exception ex)
            {
                Debug.Print($"Could not get source file: {ex.Message}");
            }

            if (sourceFile != null && sourceFile.SourceFile != null)
            {
                Debug.Print($"Source file mime type: {sourceFile.MimeType}");

                File.WriteAllBytes(filePath, sourceFile.SourceFile);
            }
            else
            {
                // If no source file, get doc file as multipage tif
                filePath = ds.GetDocumentAsFile(sessionId, documentId, folderPath, OriginalFileName);
            }

            return filePath;
        }

        /// <summary>
        /// Return original filename.  In case of no original filename, "DocId-{DocIdGUID}".
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public static string OriginalFilenameOrDocId(string sessionId, string documentId)
        {
            string OriginalFileName=string.Empty;
            var ds = new CaptureDocumentService();

            try
            {
                OriginalFileName = ds.GetTextExtension(sessionId, null, documentId, "Kofax.CEBPM.FileName");
            }
            catch (Exception ex)
            {
                Debug.Print($"Error getting original name: {ex.Message}");
            }

            if (string.IsNullOrWhiteSpace(OriginalFileName)) {
                OriginalFileName = "DocId-" + documentId;
            }

            return OriginalFileName;
        }

        /// <summary>
        /// Saves the document as a multipage tiff with original filename (doc GUID if no name)
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="documentId"></param>
        /// <param name="folderPath"></param>
        /// <param name="AppendToFilename"></param>
        /// <returns></returns>
        public string SaveTiff(string sessionId, string documentId, string folderPath, string AppendToFilename)
        {
            string FileName = OriginalFilenameOrDocId(sessionId, documentId);
            if (!String.IsNullOrWhiteSpace(AppendToFilename))
            {
                FileName = $"{Path.GetFileNameWithoutExtension(FileName)} - {AppendToFilename}";
            }
            var ds = new CaptureDocumentService();
            string filePath;
            try
            {
                filePath = ds.GetDocumentAsFile(sessionId, documentId, folderPath, FileName);
            }
            catch (Exception ex)
            {
                filePath = Path.Combine(folderPath, $"{FileName}-error.txt");
                File.WriteAllText(filePath, ex.ToString());
            }

            return filePath;
        }

        /// <summary>
        /// Calls SaveSourceFile for each document in a folder and returns an array with the resulting 
        /// file paths.  Use SaveSourceFilesOpmt for an OPMT system.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="folderId"></param>
        /// <param name="folderPath"></param>
        /// <returns>array with the resulting file paths.  Suitable for mapping to a dynamic complex variable with one string column,
        /// and mapping to the attachment field of an email activity.</returns>
        public string[] SaveSourceFiles(string sessionId, string folderId, string folderPath)
        {
            var ds = new CaptureDocumentService();
            var folder = ds.GetFolder(sessionId, null, folderId);
            var files = new List<string>();

            if (folder.Documents != null)
            {
                foreach (var doc in folder.Documents)
                {
                    files.Add(SaveSourceFile(sessionId, doc.Id, folderPath));
                }
            }

            return files.ToArray<string>();
        }

        /// <summary>
        /// OPMT can only send email from the file system from the specific tenant's generated documents path.  The email activity will
        /// add that path when trying to get the file, so this function will save to that folder, then return only file names instead of 
        /// full paths.  That way the return value can be used directly in the email activity's attachment field.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="folderId"></param>
        /// <param name="tenantName"></param>
        /// <param name="live">True if live, false if dev</param>
        /// <returns>array with the resulting file names.  Suitable for mapping to a dynamic complex variable with one string column,
        /// and mapping to the attachment field of an email activity.</returns>
        public string[] SaveSourceFilesOpmt(string sessionId, string folderId, string tenantName, bool live)
        {
            var ds = new CaptureDocumentService();
            var folder = ds.GetFolder(sessionId, null, folderId);
            var files = new List<string>();

            string tenant = tenantName + "_" + (live ? "live" : "dev");
            string folderPath = @"C:\ProgramData\Kofax\TotalAgility\Tenants\" + tenant + @"\Generated Documents\";

            if (folder.Documents != null)
            {
                foreach (var doc in folder.Documents)
                {
                    string filePath = SaveSourceFile(sessionId, doc.Id, folderPath);
                    var file = new FileInfo(filePath);
                    // just add the file name within the OPMT generated docs folder for the given tenant and "liveness"
                    files.Add(file.Name);
                }
            }

            return files.ToArray<string>();
        }
    }
}
