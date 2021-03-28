/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.csv
{
    /// <summary>
    /// [csv2lambda] slot for transforming from CSV to a lambda object.
    /// </summary>
    [Slot(Name = "csv2lambda")]
    public class Csv2Lambda : ISlot
    {
        /// <summary>
        /// Slot implementation.
        /// </summary>
        /// <param name="signaler">Signaler that raised signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            // Getting raw CSV text, and making sure we remove any expressions or value in identity node.
            var csv = input.GetEx<string>();
            input.Value = null;

            // Reading through CSV file.
            using (var reader = new StringReader(csv))
            {
                using (var parser = new CsvParser(reader, CultureInfo.InvariantCulture))
                {
                    // Buffer for column names, assuming CSV file contains headers.
                    var columns = new List<string>();

                    // Reading through each record in CSV file.
                    var first = true;
                    while (parser.Read())
                    {
                        // Checking if we're reading headers.
                        if (first)
                        {
                            // Header row.
                            first = false;
                            columns.AddRange(parser.Record);
                        }
                        else
                        {
                            // Normal record.
                            var cur = new Node(".");
                            for (var idx = 0; idx < columns.Count; idx++)
                            {
                                cur.Add(new Node(columns[idx], parser.Record[idx]));
                            }
                            input.Add(cur);
                        }
                    }
                }
            }
        }
    }
}
