using System.Collections.Generic;

namespace Ttf.Server.Core.Contracts
{
    public struct Input
    {
        public bool A, B, C;
        public int D, E, F;

        public Dictionary<string, object> GetCalcParameters()
        {
            return new Dictionary<string, object> {{nameof(D), D}, {nameof(E), E}, {nameof(F), F}};
        }

        public Dictionary<string, object> GetRuleParameters()
        {
            return new Dictionary<string, object> {{nameof(A), A}, {nameof(B), B}, {nameof(C), C}};
        }

        public static bool TryParse(string options, string d, string e, string f, out Input input)
        {
            input = new Input();
            int ve, vd, vf;

            if (!int.TryParse(d, out vd))
                return false;

            if (!int.TryParse(e, out ve))
                return false;

            if (!int.TryParse(f, out vf))
                return false;

            if (options == null || options.Length < 3)
                return false;

            input.A = options[0] == '1';
            input.B = options[1] == '1';
            input.C = options[2] == '1';
            input.D = vd;
            input.E = ve;
            input.F = vf;

            return true;
        }

        public override string ToString()
        {
            return $"{nameof(A)}: {A}, {nameof(B)}: {B}, {nameof(C)}: {C}, {nameof(D)}: {D}, {nameof(E)}: {E}, {nameof(F)}: {F}";
        }
    }
}