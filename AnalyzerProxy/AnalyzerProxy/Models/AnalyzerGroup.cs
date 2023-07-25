namespace AnalyzerProxy.Models
{
    public class AnalyzerGroup
    {
        public AnalyzerGroup()
        {

        }
        public string? GroupNameList { get; set; }
        public List<SearchList> SearchList { get; set; }
        public string? Name { get; set; }
        public string? URLPath { get; set; }
    }

    public class QueryDetail
    {
        public string? FoundName { get; set; }
        public string? FoundIdNumber { get; set; }
        public string? Offense { get; set; }
        public object ListName { get; set; }
        public string? Link { get; set; }
        public string? Zone { get; set; }
        public string? MoreInfo { get; set; }
        public string? SearchValue { get; set; }
        public string? UserCreator { get; set; }
        public DateTime CreateDate { get; set; }

        public List<Actuacion> actuaciones { get; set; }
    }

    public class Actuacion
    {
        public Actuacion()
        {

        }

        public string fecha { get; set; }
        public string tipoActuacion { get; set; }
        public string cuaderno { get; set; }
        public string folio { get; set; }
    }

    public class SearchList
    {
        public SearchList()
        {

        }
        public string? ListName { get; set; }
        public bool InRisk { get; set; }
        public QueryDetail QueryDetail { get; set; }
    }
}
