using projectmap.ViewModel;

namespace projectmap.Common
{
    public static class RanDomCode
    {
        public static string geneAction(int length)
        {
            var random = new Random();
            string code = Status.RANDOMCODE;
            var geneCode = new string(Enumerable.Repeat(code, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return geneCode;
        }

        public static int geneActionInteGer(int length)
        {
            var random = new Random();
            string code = Status.RANDOMCODEINTEGER;
            var geneCode = new string(Enumerable.Repeat(code, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return int.Parse(geneCode);
        }
    }
}
