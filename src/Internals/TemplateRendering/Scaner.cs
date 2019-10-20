using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Rapidity.Http
{
    /// <summary>
    /// todo：需要重新设计结构
    /// </summary>
    internal class Scaner
    {
        private char[] _startMark;
        private char[] _endMark;

        public Scaner(string startMark, string endMark)
        {
            this._startMark = startMark.ToCharArray();
            this._endMark = endMark.ToCharArray();
        }

        public IEnumerable<Variable> Scan(string template)
        {
            if (string.IsNullOrWhiteSpace(template)) return null;
            var variables = new Collection<Variable>();
            StringBuilder sb = new StringBuilder();

            bool flag = false;
            bool inMark = false;
            int markIndex = 0;
            StringBuilder tempBuilder = new StringBuilder();

            for (int index = 0; index < template.Length; index++)
            {
                var current = template[index];
                if (flag)
                {
                    if (current != _endMark[markIndex])
                    {
                        sb.Append(current);
                        if (markIndex != 0) markIndex = 0;
                    }
                    else
                    {
                        tempBuilder.Append(current);
                        if (!inMark && markIndex == 0) inMark = true;
                        if (markIndex == _endMark.Length - 1)
                        {
                            //此处闭合

                        }
                        markIndex++;
                    }

                }
                if (current == _startMark[markIndex])
                {
                    if (!inMark && markIndex == 0) inMark = true;
                    if (markIndex == _startMark.Length - 1)
                    {
                        if (flag) throw new InvalidOperationException("模板变量当前不支持嵌套");
                        inMark = false;
                        markIndex = 0;
                        flag = true;
                        continue;
                    }
                    markIndex++;
                    continue;
                }
            }

            return variables;
        }
    }
}