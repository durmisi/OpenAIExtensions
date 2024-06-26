﻿namespace OpenAIExtensions.Plugins.Text2Sql
{
    public class TableInformation
    {
        public string Name { get; set; }

        public string Schema { get; set; }

        public IEnumerable<ColumnInformation> Columns { get; set; }

        public TableInformation()
        {
            Columns = [];
        }
    }
}