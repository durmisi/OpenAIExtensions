﻿namespace OpenAIExtensions.Text2Sql
{
    public class ContextInformation
    {
        public List<TableInformation> Tables { get; set; }

        public ContextInformation()
        {
            Tables = [];
        }
    }
}