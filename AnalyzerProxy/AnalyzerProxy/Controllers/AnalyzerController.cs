using AnalyzerProxy.Models;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using System.Collections.Generic;
using System.Net;

namespace AnalyzerProxy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyzerController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<AnalyzerController> _logger;

        public AnalyzerController(ILogger<AnalyzerController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public List<AnalyzerGroup> Get(int IdentificationType, string SearchParam)
        {
            try
            {
                var urlRequest = this._configuration["KeysAnalyzer:UrlAnalyzer"];
                var token = this._configuration["KeysAnalyzer:Token"];
                var timeout = this._configuration["KeysAnalyzer:TimeOut"];


                #region Generate Request


                string url = urlRequest + "?IdentificationType=" + IdentificationType + "&SearchParam=" + SearchParam + "&Token=" + token;

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", token);
                request.Timeout = Convert.ToInt32(timeout);

                var response = (HttpWebResponse)request.GetResponse();
                var result = new ResponseHttpResult();

                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        result.JsonResponse = sr.ReadToEnd();
                        result.StatusCode = response.StatusCode.ToString();
                        result.Token = token;
                    }
                }

                //Deserealize response
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var jsonDeserializer = json_serializer.DeserializeObject(result.JsonResponse);
                List<AnalyzerGroup> queryResult = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AnalyzerGroup>>(jsonDeserializer.ToString());

                //Remove national and international list
                List<AnalyzerGroup> oResultFilter = queryResult.Where(x => x.GroupNameList != "LISTAS RESTRICTIVAS, SANCIONES NACIONALES E INTERNACIONALES").ToList();

                #endregion

                #region Filter List


                var group = queryResult.Where(x => x.GroupNameList == "LISTAS RESTRICTIVAS, SANCIONES NACIONALES E INTERNACIONALES").FirstOrDefault();
                List<AnalyzerGroup> oReturn = new List<AnalyzerGroup>();

                AnalyzerGroup nationalList = new AnalyzerGroup()
                {
                    GroupNameList = "LISTAS RESTRICTIVAS, SANCIONES NACIONALES",
                    SearchList = new List<SearchList>()
                };
                AnalyzerGroup internationalList = new AnalyzerGroup()
                {
                    GroupNameList = "LISTAS RESTRICTIVAS, SANCIONES INTERNACIONALES",
                    SearchList = new List<SearchList>()
                };

                //Load tipification file
                var jsonFilter = this._configuration["KeysAnalyzer:ListJsonFile"];


                //json to obj
                List<GroupFilter> listFiltered = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GroupFilter>>(jsonFilter);
                GroupFilter listSelected;



                listFiltered.Where(x => !x.internacional)?.All(x =>
                {

                    var list = group.SearchList.Where(s => s.ListName.Contains(x.nombre)).FirstOrDefault();

                    if (list != null)
                    {
                        nationalList.SearchList.Add(list);
                    }
                    return true;
                });

                listFiltered.Where(x => x.internacional)?.All(x =>
                {

                    var list = group.SearchList.Where(s => s.ListName.Contains(x.nombre)).FirstOrDefault();

                    if (list != null)
                    {
                        internationalList.SearchList.Add(list);
                    }
                    return true;
                });



                //group.SearchList?.All(x =>
                //{

                //    listSelected = listFiltered.Where(l => l.nombre.Contains(x.ListName)).FirstOrDefault();

                //    if (listSelected != null)
                //    {
                //        if (listSelected.internacional)
                //        {
                //            internationalList.SearchList.Add(x);
                //        }
                //        else
                //        {
                //            nationalList.SearchList.Add(x);
                //        }
                //    }
                    

                //    return true;
                //});

                oResultFilter.Add(nationalList);
                oResultFilter.Add(internationalList);


                #endregion

                return oResultFilter;
            }
            catch (Exception ex)
            {

                throw;
            }


            



        }


    }
}