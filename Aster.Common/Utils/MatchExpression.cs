using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Aster.Common.Utils
{
    public class MatchExpression
    {
        public List<Regex> Regexes { get; set; }

        public Action<System.Text.RegularExpressions.Match, object> Action { get; set; }
    }
}
