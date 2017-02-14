using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

namespace EtrApiSample
{
    public class EtrApiSample
    {
        public  string EtrApiTest()
        {
            using (var client = new HttpClient())
            {
                var result = "<table><tr><th>Planned Procurement</th><th>Tenders</th><th>Contracts</th></tr>";

                var baseUrl = "http://etr-aws.gruden.com/";

                var jsonResponse = getAndDecode(baseUrl + "?event=public.api.planning.search&ResultsPerPage=99");

                foreach (var release in jsonResponse.releases)
                {
                   
                    var ppUuid = release.tender.plannedProcurementUUID;
                    var plannedProcurement = getAndDecode(baseUrl + "?event=public.api.planning.view&plannedProcurementUUID=" + ppUuid);
                    var rftArray = release.tender.relatedRFT?.ToObject<string[]>();
                    var rftCount = rftArray != null ? rftArray.Length: 1;
                    var ppTender = plannedProcurement.releases[0].tender;
                    result += "<tr><td rowspan='"+ rftCount  + "'><a href=\"" + baseUrl + "?event=public.api.planning.view&plannedProcurementUUID=" + ppUuid + "\">";
                    result += ppTender.id + "</a><br/>Estimated Date of Approach to Market:" + ppTender.estimatedDateToMarket + "</td>";
                    
                    if (plannedProcurement.releases[0].tender.relatedRFT != null)
                    {
                        var rftIndex = 0;
                        foreach (var rftUuid in rftArray)
                        {
                            result += "<td>";
                            var rft = getAndDecode(baseUrl + "?event=public.api.tender.view&RFTUUID=" + rftUuid);
                            var rftTender = rft.releases[0].tender;
                            result += "<a href=\"" + baseUrl + "?event=public.rft.show&RFTUUID=" + rftUuid + "\">";
                            result += rftTender.title + "</a><br/>Published:"+ rftTender.tenderPeriod.startDate + "</td>";
                            result += "<td>";
                            if (rftTender.relatedCN != null)
                            {
                               
                                foreach(var cnUuid in rftTender.relatedCN)
                                {
                                    var contract = getAndDecode(baseUrl + "?event=public.api.contract.view&CNUUID=" + cnUuid);
                                    result += "<a href='" + baseUrl + "?event=public.cn.view&CNUUID=" + cnUuid + "'>";
                                    result += contract.releases[0].awards[0].title + "</a><br/>";
                                }
                            }
                            if (rftTender.relatedSON != null)
                            {
                                foreach (var sonUuid in rftTender.relatedSON)
                                {
                                    var contract = getAndDecode(baseUrl + "?event=public.api.contract.view&SONUUID=" + sonUuid
                                        );
                                    result += "<a href='" + baseUrl + "?event=public.SON.view&SONUUID=" + sonUuid + "'>";
                                    result += contract.releases[0].awards[0].title + "</a><br/>";
                                }
                               
                            }
                            result += "</td>";
                            
                            if (rftIndex != rftCount - 1)
                            {
                                result += "</tr><tr>";
                            }
                            rftIndex++;

                        }
                    }
                    else
                    {
                        result += "<td>No Tenders</td></td><td>";
                    }
                    result += "</tr>";
                }
                result += "</table>";


                return result;
            }
        }

        private dynamic getAndDecode(string url)
        {
            using (var client = new HttpClient())
            {
                var result = "";
                var response = client.GetStringAsync(url);
                try
                {
                    result = response.Result;
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception Message: {0}",e.Message);
                }
                
                return JsonConvert.DeserializeObject<dynamic>(result);
            }
            
        }
    }
}
