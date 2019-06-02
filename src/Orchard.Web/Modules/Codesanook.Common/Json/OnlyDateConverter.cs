using Newtonsoft.Json.Converters;

namespace CodeSanook.Common.Json
{
    public class OnlyDateConverter : IsoDateTimeConverter
    {
        public OnlyDateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }

}
