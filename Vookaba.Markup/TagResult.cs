using System;

namespace Vookaba.Markup
{
    public class TagResult
    {
        private static readonly TagResult _failResult = new();

        public static TagResult Fail => _failResult;

        public bool Successed { get; init; }

        public int Length { get; private init; }

        public ReadOnlyMemory<char> Content { get; private init; }

        public string OpeningHtml { get; private init; } = "";

        public string ClosingHtml { get; private init; } = "";

        private TagResult() { }

        public static TagResult Found(
            int length,
            ReadOnlyMemory<char> content,
            string opening,
            string closing) => new()
            {
                Successed = true,
                Length = length,
                Content = content,
                OpeningHtml = opening,
                ClosingHtml = closing
            };
    }












    //class PostTag : IMarkupTag
    //{
    //    public override TagResult TryOpen(ReadOnlySpan<char> msg)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override Task<TagResult> TryOpenAsync(ReadOnlyMemory<char> m)
    //    {
    //        var s = m.Span;
    //        if (s.Length > 2 && s.StartsWith(">>"))
    //        {
    //            int prsIdx = 2;
    //            var brd = GetBoardKey(s, ref prsIdx);
    //            var pst = GetPostId(s, ref prsIdx);
    //            if (pst.Length > 0 && TryCreateTags(brd, pst, out var ooo, out var ccc))
    //            {
    //                return Task.FromResult(TagResult.Found(
    //                    prsIdx,
    //                    new Rng(2 + brd.Length + (brd.Length == 0 ? 0 : 1), prsIdx),
    //                    //new Rng(2, prsIdx), 
    //                    ooo,
    //                    ccc));
    //            }
    //        }
    //        return Task.FromResult(TagResult.Fail);
    //    }

    //    private ReadOnlySpan<char> GetBoardKey(ReadOnlySpan<char> s, ref int pi)
    //    {
    //        var i = pi;
    //        while (s[i] >= 'a' && s[i] <= 'z') i++;
    //        if (i != pi)
    //        { //board id found
    //            if (s[i] == '/') { var bk = s.Slice(pi, i - pi).ToString(); pi = i + 1; return bk; }
    //        }
    //        return ReadOnlySpan<char>.Empty;
    //    }

    //    private string GetCurBrd() => "curr";

    //    private ReadOnlySpan<char> GetPostId(ReadOnlySpan<char> s, ref int pi)
    //    {
    //        var i = pi;
    //        while (pi < s.Length && char.IsDigit(s[i])) pi++;
    //        return s.Slice(i, pi - i);
    //    }

    //    private bool TryCreateTags(ReadOnlySpan<char> brd, ReadOnlySpan<char> pst, out string tag1, out string tag2)
    //    {
    //        tag1 = ""; tag2 = "";
    //        int pnum; string brdKey; string cur = GetCurBrd();
    //        if (!int.TryParse(pst, out pnum)) return false;
    //        brdKey = brd.Length == 0 ? cur : brd.ToString();
    //        tag1 = $"<a href = \"{brdKey}\\{pnum}\"> <span style=\"color:red;\">&gt&gt";
    //        tag2 = brdKey == cur ? "</span></a>" : $" (/{brdKey}/)</span></a>";
    //        return true;
    //    }
    //}
}
