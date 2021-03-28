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
            var builder = new StringBuilder();
            var first = true;
            foreach (var idx in input.Evaluate())
            {
                if (first)
                {
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
                var firstValue = true;
                foreach (var idxValue in idx.Children)
                {
                    if (firstValue)
                        firstValue = false;
                    else
                        builder.Append(",");
                    var value = idxValue.Value;
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
            input.Value = builder.ToString();
        }
    }
}
