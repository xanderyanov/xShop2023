﻿namespace MirchasovStore
{
    internal class StrUtils
    {
        public static void Split(string S, char Separator, char Quotator, List<string> List)
        {
            List.Clear();
            if (S == null) return;
            char PrevChar = '\0';
            bool Quoted = false;
            string V = string.Empty;
            for (int i = 0; i < S.Length; i++)
            {
                char c = S[i];
                if (c == Quotator)
                {
                    Quoted = !Quoted;
                    if (Quoted && (PrevChar == Quotator)) V += Quotator;
                }
                else
                {
                    if (c == Separator)
                    {
                        if (Quoted)
                        {
                            V += c;
                        }
                        else
                        {
                            List.Add(V);
                            V = string.Empty;
                        }
                    }
                    else
                    {
                        V += c;
                    }
                }
                PrevChar = c;
            }
            List.Add(V);
        }

        public static List<string> Split(string S, char Separator, char Quotator)
        {
            List<string> List = new List<string>();
            Split(S, Separator, Quotator, List);
            return List;
        }
    }
}
