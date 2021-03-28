/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Text;
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

            // Looping through each node we should transform to a CSV record.
            var first = true;
            foreach (var idx in input.Evaluate())
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
                    }
                    builder.Append("\r\n");
                }

                // Looping through each child node of currently iterated record, to create our cells.
                var firstValue = true;
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
                        else
                            builder.Append(Converter.ToString(value).Item2);
                    }
                }
                builder.Append("\r\n");
            }

            // Returning CSV content to caller.
            input.Value = builder.ToString();
        }
    }
}
