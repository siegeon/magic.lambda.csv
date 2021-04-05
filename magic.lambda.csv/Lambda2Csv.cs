/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.csv
{
    /// <summary>
    /// [lambda2csv] slot for transforming a lambda hierarchy to a CSV string.
    /// </summary>
    [Slot(Name = "lambda2csv")]
    public class Lambda2Csv : ISlot
    {
        /// <summary>
        /// Slot implementation.
        /// </summary>
        /// <param name="signaler">Signaler that raised signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            // Buffer to keep CSV data.
            var builder = new StringBuilder();

            // List containing type information.
            var types = new List<Tuple<string, string>>();

            // Looping through each node we should transform to a CSV record.
            var first = true;
            foreach (var idx in input.Value == null ? input.Children : input.Evaluate())
            {
                // Checking if this is the first record, at which point we create headers for CSV file.
                if (first)
                {
                    // Creating CSV headers.
                    first = false;
                    var firstHeader = true;
                    foreach (var idxHeader in idx.Children)
                    {
                        if (firstHeader)
                            firstHeader = false;
                        else
                            builder.Append(",");
                        builder.Append(idxHeader.Name);
                        types.Add(new Tuple<string, string>(idxHeader.Name, null));
                    }
                    builder.Append("\r\n");
                }

                // Looping through each child node of currently iterated record, to create our cells.
                var firstValue = true;
                var index = 0;
                foreach (var idxValue in idx.Children)
                {
                    if (firstValue)
                        firstValue = false;
                    else
                        builder.Append(",");
                    var value = idxValue.Value;

                    // Making sure we escape string values correctly.
                    if (!(value is null))
                    {
                        if (value is string)
                            builder.Append("\"" + idxValue.GetEx<string>().ToString().Replace("\"", "\"\"") + "\"");
                        else if (value == null)
                            builder.Append("[NULL]");
                        else
                            builder.Append(Converter.ToString(value).Item2);
                        if (types[index].Item2 == null)
                            types[index] = new Tuple<string, string>(types[index].Item1, Converter.ToString(value).Item1);
                    }
                    index ++;
                }
                builder.Append("\r\n");
            }

            // Returning CSV content to caller.
            input.Value = builder.ToString();
            input.AddRange(types.Select(x => new Node(x.Item1, x.Item2)));
        }
    }
}
