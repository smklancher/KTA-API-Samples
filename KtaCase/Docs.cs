using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalAgility.Sdk;
using Agility.Sdk.Model;
using Agility.Sdk.Model.Capture;
using System.Diagnostics;

namespace KtaCase
{
    public class Docs
    {

        public string Details(string sessionId, string documentId)
        {
            var s = new StringBuilder($"Id: {documentId}");
            var ds = new CaptureDocumentService();
            var d = ds.GetDocument(sessionId, null, documentId);

            s.AppendLine();
            s.AppendLine(ReflectionHelper.PropertyList(d));
            s.AppendLine();

            if (d.Fields != null)
            {
                foreach (var f in d.Fields)
                {
                    s.AppendLine(ReflectionHelper.PropertyList(f));
                }
            }

            s.AppendLine();
            s.AppendLine($"Pages: {d.NumberOfPages}");
            s.AppendLine();
            int pageIndex = 0;
            foreach(var p in d.Pages)
            {
                s.AppendLine($"Page {pageIndex + 1} of {d.NumberOfPages}");
                s.AppendLine(ReflectionHelper.PropertyList(p));
                s.AppendLine();

                if (p.Annotations != null)
                {
                    foreach (var a in p.Annotations)
                    {
                        s.AppendLine($"Page {a.PageIndex}: {a.Text} ({a.Author}, {a.Timestamp?.ToString() ?? "Unknown Time"}");
                    }
                }

                if (p.Barcodes != null)
                {
                    foreach (var b in p.Barcodes)
                    {
                        s.AppendLine(ReflectionHelper.PropertyList(b));
                    }
                }

                if (p.Words != null)
                {
                    s.AppendLine("Words property");
                    foreach (var w in p.Words)
                    {
                        s.AppendLine(ReflectionHelper.PropertyList(w));
                    }
                }

                //// See available properties: https://docshield.kofax.com/KTA/en_US/750-4kcae04o43/help/API/latest/class_agility_1_1_sdk_1_1_services_1_1_capture_document_service.html#a371ac5b01657cab2d46623c7f43f84b4
                //var ppic = new PagePropertiesIdentityCollection(){
                //    // Could request properties from more than one page here if needed
                //    new PagePropertiesIdentity(p.Id,  new FieldSystemPropertyIdentityCollection() {
                //        new FieldSystemPropertyIdentity() { Name = "Words" }
                //    })
                //};

                //var pageProperties = ds.GetPagePropertyValues(sessionId, documentId, ppic);

                WordCollection words=DocPageWords(sessionId,documentId,pageIndex);
                //foreach(var pageProps in pageProperties)
                //{
                //    words=(WordCollection)pageProps.PropertyCollection.Where(x => x.SystemFieldIdentity.Name == "Words").FirstOrDefault()?.Value;
                    if (words != null)
                    {
                        s.AppendLine("GetPagePropertyValues-Words property");
                        foreach (var w in words)
                        {
                            s.AppendLine($"{w.Text}");
                        }
                    }
                //}

                pageIndex++;
            }
            

            return s.ToString();
        }

        /// <summary>
        /// Returns collection of Word objects from given document and page index (0-based)
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="documentId"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public WordCollection DocPageWords(string sessionId, string documentId, int pageIndex)
        {
            var ds = new CaptureDocumentService();
            Document d = ds.GetDocument(sessionId, null, documentId);

            // If needed, it is nicer to return an empty collection instead of null
            WordCollection words = new WordCollection();

            // index is 0 based, Number is 1 based
            if (pageIndex >= d.NumberOfPages)
            {
                // Could throw exception here
                Debug.WriteLine("Page index {pageIndex} does not exist!");
                return words;
            }

            Page p = d.Pages[pageIndex];

            // Build the request for specific properties from specific pages
            // See available properties: https://docshield.kofax.com/KTA/en_US/750-4kcae04o43/help/API/latest/class_agility_1_1_sdk_1_1_services_1_1_capture_document_service.html#a371ac5b01657cab2d46623c7f43f84b4
            var ppic = new PagePropertiesIdentityCollection(){
                // Could request properties from more than one page here if needed
                new PagePropertiesIdentity(p.Id,  new FieldSystemPropertyIdentityCollection() {
                    new FieldSystemPropertyIdentity() { Name = "Words" }
                })
            };

            PagePropertiesCollection pageProperties = ds.GetPagePropertyValues(sessionId, documentId, ppic);

            // Property object per page (although in this case, only requesting from a single page)
            foreach (PageProperties pageProps in pageProperties)
            {
                words = (WordCollection)pageProps.PropertyCollection.Where(x => x.SystemFieldIdentity.Name == "Words").FirstOrDefault()?.Value;
            }

            // In case of null, use empty collection
            words = words ?? new WordCollection();

            return words;
        }

    }
}
