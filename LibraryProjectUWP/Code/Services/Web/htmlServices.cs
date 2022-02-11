using HtmlAgilityPack;
using LibraryProjectUWP.ViewModels.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.Code.Services.Web
{
    public class htmlServices
    {
        public async Task<LivreVM> GetBookFromAmazonAsync(Uri uri, LivreVM viewModel)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string strHTML = await httpClient.GetStringAsync(uri);

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(strHTML);

                return await GetBookFromAmazonAsync(htmlDocument, viewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<LivreVM> GetBookFromAmazonAsync(HtmlDocument htmlDocument, LivreVM viewModel)
        {
            try
            {
                if (htmlDocument == null)
                {
                    return null;
                }

                viewModel = await GenerateAmazonBookTitleAsync(htmlDocument, viewModel);

                //var htmlInformationsTab = htmlDocument.GetElementbyId("onglets_1_information");
                //if (htmlInformationsTab == null) return viewModel;

                //viewModel = await GenerateNautiljonDescriptionAsync(htmlInformationsTab, viewModel);

                //var generalInfoNode = htmlInformationsTab.Descendants("div").FirstOrDefault(f => f.HasClass("infosFicheTop"));
                //if (generalInfoNode != null)
                //{
                //    var listeinfoContainer = generalInfoNode.Descendants("div").FirstOrDefault(f => f.HasClass("liste_infos"));
                //    if (listeinfoContainer != null)
                //    {
                //        var listInfo = generalInfoNode.Descendants("ul").FirstOrDefault(f => f.HasClass("mb10"));
                //        if (listInfo != null)
                //        {
                //            var listItemsInfo = generalInfoNode.Descendants("li").ToList();
                //            if (listItemsInfo != null && listItemsInfo.Any())
                //            {
                //                viewModel = await GenerateNautiljonAlternativesTitlesAsync(listItemsInfo, viewModel);
                //                viewModel = await GenerateNautiljonOriginalsTitlesAsync(listItemsInfo, viewModel);
                //                viewModel = await GenerateNautiljonGenresAsync(listItemsInfo, viewModel);
                //                viewModel = await GenerateNautiljonGroupesAsync(listItemsInfo, viewModel);
                //                viewModel = await GenerateNautiljonFormatAsync(listItemsInfo, viewModel);
                //                viewModel = await GenerateNautiljonOrigineAdaptationAsync(listItemsInfo, viewModel);
                //                viewModel.Link = await GenerateNautiljonOfficialWebSitesAsync(listItemsInfo, viewModel.Link);
                //                viewModel.Format = await GenerateNautiljonEpisodesAsync(listItemsInfo, viewModel.Format);
                //                viewModel.Licence = await GenerateNautiljonEditeurAsync(listItemsInfo, viewModel.Licence);
                //                viewModel.Staff.PaysProduction = (await GenerateNautiljonPaysProductionAsync(listItemsInfo, viewModel.Staff))?.PaysProduction;
                //                viewModel.Staff.Societys = (await GenerateNautiljonStudioProdAsync(listItemsInfo, viewModel.Staff))?.Societys;
                //                viewModel.Diffusion = await GenerateNautiljonDateDiffusionAsync(listItemsInfo, viewModel.Diffusion);
                //                viewModel = await GenerateNautiljonSaisonAsync(listItemsInfo, viewModel);
                //            }

                //        }
                //    }
                //}
                return viewModel;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private Task<LivreVM> GenerateAmazonBookTitleAsync(HtmlDocument document, LivreVM viewModel)
        {
            try
            {
                if (document == null)
                {
                    return Task.FromResult(viewModel);
                }

                var content = document.GetElementbyId("productTitle");

                if (content == null) return Task.FromResult(viewModel);
                viewModel.MainTitle = content.InnerText?.Trim();

                return Task.FromResult(viewModel);
            }
            catch (Exception)
            {
                return Task.FromResult(viewModel);
            }
        }

    }
}
